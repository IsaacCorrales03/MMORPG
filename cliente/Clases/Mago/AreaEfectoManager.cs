using Godot;
using Shared.Magia;

public partial class AreaEfectoManager : Node
{
    [Export] private SistemaCasteo _sistemaCasteo;
    [Export] private AreaHechizoPreview _preview;
    [Export] private VisualesHechizos _visualesHechizos;
    [Export] private PackedScene _areaHechizoScene; 
    public override void _Ready()
    {
        _sistemaCasteo.CasteoCompletado += OnCasteoCompletado;
        _sistemaCasteo.CasteoCancelado += OnCasteoCancelado;
    }

    private void OnCasteoCompletado()
    {
        var hechizo = _sistemaCasteo.HechizoActual;
        if (hechizo == null) return;

        _preview.Mostrar(hechizo.Radio);
    }

    private void OnCasteoCancelado()
    {
        _preview.Ocultar();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_preview.Visible) return;

        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.Pressed &&
            mouseEvent.ButtonIndex == MouseButton.Left)
        {
            ColocarAreaHechizo();
            GetViewport().SetInputAsHandled();
        }
    }

    private void ColocarAreaHechizo()
    {
        var hechizo = _sistemaCasteo.HechizoActual;
        if (hechizo == null) return;

        var posicion = _preview.GlobalPosition;

        if (_areaHechizoScene != null)
        {
            var area = _areaHechizoScene.Instantiate<AreaHechizo>();
            GetTree().CurrentScene.AddChild(area);

            var textura = _visualesHechizos?.ObtenerTextura(hechizo.Nombre);
            area.Inicializar(hechizo.Radio, textura, posicion);
        }

        EnviarCasteoAlServidor(hechizo, posicion);

        _preview.Ocultar();
        _sistemaCasteo.FinalizarUso();
    }

    private void EnviarCasteoAlServidor(Hechizo hechizo, Vector2 posicion)
    {
        // TODO: acá va el RPC/paquete de red real, por ejemplo:
        // Rpc(nameof(SolicitarCasteo), hechizo.Nombre, posicion.X, posicion.Y);
        GD.Print($"[Red] Casteo de '{hechizo.Nombre}' en posición {posicion}");
    }
}
