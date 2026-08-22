using Godot;
using Shared.Magia;
using System;
using System.Collections.Generic;

public partial class RuedaHechizos : Control
{
    [Export] public PackedScene SlotHechizoScene;
    [Export] public Control ContenedorSlots;
    [Export] public Panel Centro;
    [Export] public Label NombreHechizoLabel;
    [Export] public Label ManaHechizoLabel;
    [Export] public Label DescripcionHechizo;
    [Export] public Label DuracionHechizoLabel;

    public event Action<Hechizo> HechizoSeleccionado;

    private SlotHechizo _slotActivo;
    private List<SlotHechizo> _slots = new();
    private float _radio = 280f;

    private const float RadioInterior = 200f;
    private const float RadioExterior = 430f;
    private const int SegmentosArco = 24;

    // --- Animación ---
    private const float DuracionAnim = 0.22f;
    private Tween _tweenActivo;
    private bool _cerrando = false;

    public void Abrir(Tomo tomo)
    {
        LimpiarSlots();
        int cantidad = tomo.EspacioMaximo;
        Vector2 centro = Size / 2;

        for (int i = 0; i < cantidad; i++)
        {
            var slotInstance = SlotHechizoScene.Instantiate<SlotHechizo>();
            ContenedorSlots.AddChild(slotInstance);
            slotInstance.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
            slotInstance.Size = new Vector2(120, 120);

            float angulo = Mathf.Tau / cantidad * i - Mathf.Pi / 2;
            Vector2 pos = centro + new Vector2(Mathf.Cos(angulo) * _radio, Mathf.Sin(angulo) * _radio);
            slotInstance.Position = pos - slotInstance.Size / 2;

            if (i < tomo.Hechizos.Count)
                slotInstance.AsignarHechizo(tomo.Hechizos[i]);
            else
                slotInstance.SetNull();

            _slots.Add(slotInstance);
        }

        _cerrando = false;

        // Preparar estado inicial (invisible/chico) antes de mostrar
        PivotOffset = Size / 2;
        Scale = new Vector2(0.6f, 0.6f);
        Modulate = new Color(1, 1, 1, 0f);
        Visible = true;
        QueueRedraw();

        _tweenActivo?.Kill();
        _tweenActivo = CreateTween();
        _tweenActivo.SetEase(Tween.EaseType.Out);
        _tweenActivo.SetTrans(Tween.TransitionType.Back);
        _tweenActivo.TweenProperty(this, "scale", Vector2.One, DuracionAnim);
        _tweenActivo.Parallel().TweenProperty(this, "modulate:a", 1f, DuracionAnim * 0.8f);
    }

    public override void _Process(double delta)
    {
        if (!Visible || _cerrando || _slots.Count == 0) return;

        Vector2 centro = Size / 2;
        Vector2 mouseDir = GetLocalMousePosition() - centro;

        SlotHechizo nuevoActivo = null;

        if (mouseDir.Length() >= RadioInterior)
        {
            float anguloMouse = Mathf.Atan2(mouseDir.Y, mouseDir.X);
            int cantidad = _slots.Count;
            float pasoAngulo = Mathf.Tau / cantidad;
            float anguloAjustado = anguloMouse + Mathf.Pi / 2;
            if (anguloAjustado < 0) anguloAjustado += Mathf.Tau;

            int indiceCercano = Mathf.RoundToInt(anguloAjustado / pasoAngulo) % cantidad;
            var candidato = _slots[indiceCercano];
            nuevoActivo = candidato.Hechizo != null ? candidato : null;
        }

        if (nuevoActivo != _slotActivo)
        {
            _slotActivo = nuevoActivo;
            ActualizarCentro(_slotActivo?.Hechizo);
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        if (!Visible || _slots.Count == 0) return;

        Vector2 centro = Size / 2;
        int cantidad = _slots.Count;
        float pasoAngulo = Mathf.Tau / cantidad;

        for (int i = 0; i < cantidad; i++)
        {
            float anguloInicio = pasoAngulo * i - Mathf.Pi / 2 - pasoAngulo / 2;
            float anguloFin = anguloInicio + pasoAngulo;

            var puntos = new List<Vector2>();

            for (int s = 0; s <= SegmentosArco; s++)
            {
                float t = Mathf.Lerp(anguloInicio, anguloFin, s / (float)SegmentosArco);
                puntos.Add(centro + new Vector2(Mathf.Cos(t), Mathf.Sin(t)) * RadioExterior);
            }
            for (int s = SegmentosArco; s >= 0; s--)
            {
                float t = Mathf.Lerp(anguloInicio, anguloFin, s / (float)SegmentosArco);
                puntos.Add(centro + new Vector2(Mathf.Cos(t), Mathf.Sin(t)) * RadioInterior);
            }

            bool tieneHechizo = _slots[i].Hechizo != null;
            bool esActivo = _slots[i] == _slotActivo;

            Color color = !tieneHechizo
                ? new Color("#1B1611", 0.35f)
                : esActivo
                    ? new Color("#D4A24C", 0.55f)
                    : new Color("#1B1611", 0.8f);

            var arr = puntos.ToArray();
            DrawColoredPolygon(arr, color);

            for (int p = 0; p < arr.Length; p++)
            {
                Vector2 a = arr[p];
                Vector2 b = arr[(p + 1) % arr.Length];
                DrawLine(a, b, new Color("#5C4B33"), 1.5f);
            }
        }
    }

    private void ActualizarCentro(Hechizo hechizo)
    {
        NombreHechizoLabel.Text = hechizo?.Nombre ?? "";
        ManaHechizoLabel.Text = hechizo?.CostoMana.ToString() ?? "";
        DescripcionHechizo.Text = hechizo?.Descripcion ?? "";

        if (hechizo != null)
            DuracionHechizoLabel.Text = $"{hechizo.TiempoMinimo:0.0}s – {hechizo.TiempoMaximo:0.0}s";
        else
            DuracionHechizoLabel.Text = "";
    }

    public void LimpiarCentro()
    {
        NombreHechizoLabel.Text = "";
        ManaHechizoLabel.Text = "";
        DescripcionHechizo.Text = "";
    }

    private void LimpiarSlots()
    {
        foreach (var slot in _slots)
            slot.QueueFree();
        _slots.Clear();
    }

    public void Cerrar()
    {
        if (_cerrando) return; // evitar doble-cierre mientras anima
        _cerrando = true;

        if (_slotActivo != null)
            HechizoSeleccionado?.Invoke(_slotActivo.Hechizo);

        PivotOffset = Size / 2;

        _tweenActivo?.Kill();
        _tweenActivo = CreateTween();
        _tweenActivo.SetEase(Tween.EaseType.In);
        _tweenActivo.SetTrans(Tween.TransitionType.Back);
        _tweenActivo.TweenProperty(this, "scale", new Vector2(0.6f, 0.6f), DuracionAnim);
        _tweenActivo.Parallel().TweenProperty(this, "modulate:a", 0f, DuracionAnim);
        _tweenActivo.TweenCallback(Callable.From(FinalizarCierre));
    }

    private void FinalizarCierre()
    {
        Visible = false;
        LimpiarSlots();
        _slotActivo = null;
        LimpiarCentro();
        Scale = Vector2.One;
        Modulate = new Color(1, 1, 1, 1);
        _cerrando = false;
        QueueRedraw();
    }
}