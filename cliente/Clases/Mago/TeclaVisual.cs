using Godot;

public partial class TeclaVisual : Control
{
    [Export] public Label _letra;
    [Export] public Panel _panel;

    public enum EstadoTecla { Pendiente, Correcta, Fallada }

    private const float DuracionShake = 0.28f;
    private const float DuracionPulso = 0.18f;

    private Tween _tweenEfecto;

    public void Configurar(Tecla tecla)
    {
        _letra.Text = tecla.ToString();
        SetEstado(EstadoTecla.Pendiente);
    }

    public void SetEstado(EstadoTecla estado)
    {
        Color color = estado switch
        {
            EstadoTecla.Correcta => new Color("#4CD473"), // verde
            EstadoTecla.Fallada => new Color("#D44C4C"),  // rojo
            _ => new Color("#5C4B33")                       // pendiente, ámbar apagado
        };

        var style = new StyleBoxFlat();
        style.BgColor = new Color("#1B1611");
        style.BorderColor = color;
        style.SetBorderWidthAll(estado == EstadoTecla.Pendiente ? 1 : 3);
        style.SetCornerRadiusAll(8);
        _panel.AddThemeStyleboxOverride("panel", style);

        if (estado == EstadoTecla.Correcta)
            Pulso();
        else if (estado == EstadoTecla.Fallada)
            Shake();
    }

    public void Shake()
    {
        _tweenEfecto?.Kill();

        Vector2 posOriginal = Position;
        _tweenEfecto = CreateTween();
        _tweenEfecto.SetTrans(Tween.TransitionType.Sine);

        int pasos = 5;
        float amplitud = 8f;
        for (int i = 0; i < pasos; i++)
        {
            float dir = (i % 2 == 0) ? 1 : -1;
            float atenuacion = 1f - (float)i / pasos;
            _tweenEfecto.TweenProperty(this, "position:x", posOriginal.X + dir * amplitud * atenuacion,
                DuracionShake / pasos);
        }
        _tweenEfecto.TweenProperty(this, "position:x", posOriginal.X, DuracionShake / pasos);
    }

    public void Pulso()
    {
        _tweenEfecto?.Kill();

        PivotOffset = Size / 2;
        _tweenEfecto = CreateTween();
        _tweenEfecto.SetTrans(Tween.TransitionType.Back);
        _tweenEfecto.SetEase(Tween.EaseType.Out);
        _tweenEfecto.TweenProperty(this, "scale", new Vector2(1.25f, 1.25f), DuracionPulso * 0.4f);
        _tweenEfecto.TweenProperty(this, "scale", Vector2.One, DuracionPulso * 0.6f);
    }
}