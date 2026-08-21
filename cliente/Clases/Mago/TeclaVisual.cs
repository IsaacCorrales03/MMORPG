using Godot;

public partial class TeclaVisual : Control
{
    [Export] public Label _letra;
    [Export] public Panel _panel;

    public enum EstadoTecla { Pendiente, Correcta, Fallada }

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
    }
}