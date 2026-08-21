using Godot;
using Shared.Magia;

public partial class InputManager : Node
{
    [Export] private TomosManager _tomosManager;
    [Export] private RuedaHechizos _ruedaHechizos;
    [Export] private SistemaCasteo _sistemaCasteo;
    private int _motionEventsRecibidos = 0;
    public override void _Ready()
    {
        _ruedaHechizos.HechizoSeleccionado += OnHechizoSeleccionado;
    }

    public override void _Input(InputEvent @event)
    {
        if (_sistemaCasteo != null && _sistemaCasteo.BloqueaOtrosInputs)
            return; // nada se procesa mientras castea: ni Q, ni click de rueda


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
            {
                _ruedaHechizos?.Abrir(_tomosManager.TomoActual);
            }
            else
            {
                _ruedaHechizos?.Cerrar();
            }
        }
    }


    private void CambiarTomo()
    {
        int siguienteTomo = _tomosManager.TomoSeleccionado == 1 ? 2 : 1;
        _tomosManager.SeleccionarTomo(siguienteTomo);
    }
    private void OnHechizoSeleccionado(Hechizo hechizo)
    {
        if (hechizo == null) return;
       _sistemaCasteo.IniciarCasteo(hechizo, 6);
    }
}