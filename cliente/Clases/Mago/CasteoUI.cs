using Godot;
using Shared.Magia;
using System.Collections.Generic;

public partial class CasteoUI : Control
{
    [Export] public SistemaCasteo SistemaCasteo;
    [Export] public PackedScene TeclaVisualScene;
    [Export] public HBoxContainer ContenedorTeclas;
    [Export] public ProgressBar BarraTiempo;
    [Export] public PanelContainer PanelContenedor;
    [Export] public Label LabelConcentrando;

    private const float MargenInferior = 60f;
    private bool _enEsperaMinima = false;
    private List<TeclaVisual> _teclasVisuales = new();

    private StyleBoxFlat _fillDefault;
    private StyleBoxFlat _fillConcentrando;
    private StyleBoxFlat _bgConcentrando;

    public override void _Ready()
    {
        SistemaCasteo.CasteoIniciado += OnCasteoIniciado;
        SistemaCasteo.TeclaCorrecta += OnTeclaCorrecta;
        SistemaCasteo.TeclaFallada += OnTeclaFallada;
        SistemaCasteo.CasteoCancelado += OnCasteoCancelado;
        SistemaCasteo.CasteoCompletado += OnCasteoCompletado;
        SistemaCasteo.EntroEnEsperaMinima += OnEntroEnEsperaMinima;

        _fillDefault = new StyleBoxFlat { BgColor = new Color("#D4A94C") };

        _fillConcentrando = new StyleBoxFlat { BgColor = new Color("#E0C23C") }; // amarillo
        _bgConcentrando = new StyleBoxFlat { BgColor = new Color("#4A4A4A") };   // gris

        Visible = false;
    }

    private void OnCasteoIniciado(Hechizo hechizo, IReadOnlyList<Tecla> secuencia)
    {
        _enEsperaMinima = false;
        MostrarParaHechizo(hechizo, secuencia);
    }

    public void MostrarParaHechizo(Hechizo hechizo, IReadOnlyList<Tecla> secuencia)
    {
        LimpiarTeclas();

        ContenedorTeclas.Visible = true;
        LabelConcentrando.Visible = false;

        BarraTiempo.AddThemeStyleboxOverride("fill", _fillDefault);
        BarraTiempo.RemoveThemeStyleboxOverride("background");

        foreach (var tecla in secuencia)
        {
            var visual = TeclaVisualScene.Instantiate<TeclaVisual>();
            ContenedorTeclas.AddChild(visual);
            visual.Configurar(tecla);
            _teclasVisuales.Add(visual);
        }

        Visible = true;
        CallDeferred(nameof(ActualizarTamano));
    }

    public override void _Process(double delta)
    {
        if (!Visible) return;
        BarraTiempo.Value = _enEsperaMinima
           ? SistemaCasteo.ProgresoEsperaMinima() * BarraTiempo.MaxValue
           : SistemaCasteo.ProgresoTiempo() * BarraTiempo.MaxValue;
    }

    private void OnTeclaCorrecta(int indice) => _teclasVisuales[indice].SetEstado(TeclaVisual.EstadoTecla.Correcta);
    private void OnTeclaFallada(int indice) => _teclasVisuales[indice].SetEstado(TeclaVisual.EstadoTecla.Fallada);

    private void OnCasteoCancelado() => Ocultar();

    private void OnCasteoCompletado() => Ocultar();

    private void OnEntroEnEsperaMinima()
    {
        _enEsperaMinima = true;

        LimpiarTeclas();
        ContenedorTeclas.Visible = false;
        LabelConcentrando.Visible = true;

        BarraTiempo.AddThemeStyleboxOverride("fill", _fillConcentrando);
        BarraTiempo.AddThemeStyleboxOverride("background", _bgConcentrando);

        CallDeferred(nameof(ActualizarTamano));
    }

    private void Ocultar()
    {
        Visible = false;
        _enEsperaMinima = false;
        ContenedorTeclas.Visible = true;
        LabelConcentrando.Visible = false;
        LimpiarTeclas();
    }

    private void LimpiarTeclas()
    {
        foreach (var t in _teclasVisuales)
            t.QueueFree();
        _teclasVisuales.Clear();
    }

    private void ActualizarTamano()
    {
        Vector2 tamano = PanelContenedor.GetCombinedMinimumSize();
        Size = tamano;

        OffsetLeft = -tamano.X / 2f;
        OffsetRight = tamano.X / 2f;
        OffsetBottom = -MargenInferior;
        OffsetTop = OffsetBottom - tamano.Y;
    }
}