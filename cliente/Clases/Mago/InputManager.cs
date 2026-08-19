using Godot;

public partial class InputManager : Node
{
    [Export] private TomosManager _tomosManager;
    [Export] private RuedaHechizos _ruedaHechizos;

    public override void _Ready()
    {
        GD.Print("Alog");
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent &&
            keyEvent.Pressed &&
            keyEvent.Keycode == Key.Q)
        {
            CambiarTomo();
        }

        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if (mouseEvent.Pressed)
                _ruedaHechizos?.AbrirRueda();
            else
                _ruedaHechizos?.CerrarYConfirmar();
        }
    }

    private void CambiarTomo()
    {
        int siguienteTomo = _tomosManager.TomoSeleccionado == 1 ? 2 : 1;
        _tomosManager.SeleccionarTomo(siguienteTomo);
    }
}