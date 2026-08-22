using Godot;

public partial class AreaHechizoPreview : Node2D
{
    private float _radio;

    // Amarillo tipo preview de LoL: relleno amarillo translúcido, borde amarillo oscuro
    private static readonly Color ColorRelleno = new(1f, 0.92f, 0.2f, 0.25f);
    private static readonly Color ColorBorde = new(0.6f, 0.5f, 0f, 0.9f);

    public override void _Ready()
    {
        Visible = false;
        ZIndex = 100; // por encima del suelo/personajes
    }

    public override void _Process(double delta)
    {
        if (!Visible) return;

        GlobalPosition = GetGlobalMousePosition();
    }

    public void Mostrar(float radio)
    {
        _radio = radio;
        Visible = true;
        QueueRedraw();
    }

    public void Ocultar()
    {
        Visible = false;
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, _radio, ColorRelleno);
        DrawArc(Vector2.Zero, _radio, 0f, Mathf.Tau, 64, ColorBorde, 3f, true);
    }
}
