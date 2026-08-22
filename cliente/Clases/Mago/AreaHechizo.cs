using Godot;

public partial class AreaHechizo : Node2D
{
    [Export] private Sprite2D _sprite;
    [Export] private Timer _timer;

    private float _radio;
    private bool _tieneTextura;

    public override void _Ready()
    {
        if (_timer != null)
            _timer.Timeout += QueueFree;
    }

    public void Inicializar(float radio, Texture2D textura, Vector2 posicion)
    {
        _radio = radio;
        GlobalPosition = posicion;

        if (textura != null && _sprite != null)
        {
            _tieneTextura = true;
            _sprite.Texture = textura;
            _sprite.Visible = true;

            // Escala el sprite para que su radio visual coincida con Hechizo.Radio
            // (asume textura cuadrada centrada, ej. 256x256 con el círculo inscripto)
            float tamañoBaseTextura = textura.GetWidth();
            float escalaDeseada = (radio * 2f) / tamañoBaseTextura;
            _sprite.Scale = new Vector2(escalaDeseada, escalaDeseada);
        }
        else if (_sprite != null)
        {
            _sprite.Visible = false;
        }

        if (_timer != null)
        {
            _timer.Start();
        }
        else
        {
            GetTree().CreateTimer(3f).Timeout += QueueFree;
        }

        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_tieneTextura) return; // el Sprite2D ya se encarga del dibujo

        // Fallback: círculo blanco si el hechizo no tiene textura asignada todavía
        DrawCircle(Vector2.Zero, _radio, new Color(1f, 1f, 1f, 0.35f));
        DrawArc(Vector2.Zero, _radio, 0f, Mathf.Tau, 64, new Color(1f, 1f, 1f, 0.9f), 2f, true);
    }
}
