using Godot;
using Shared.Magia;
using System.Collections.Generic;

public partial class RuedaHechizos : Control
{
    [Export] private TomosManager _tomosManager;
    [Export] private Control _contenedor;
    [Export] private PackedScene _escenaSlot;

    [Export] private float _radio = 150f;
    [Export] private float _anguloInicial = -90f;

    private Tomo _tomoActual;
    private readonly List<SlotHechizo> _slots = new();
    private int _indiceResaltado = -1;

    public override void _Ready()
    {
        GD.Print("Algo");
        Visible = false;
        _tomosManager.TomoCambiado += ActualizarTomo;
        ActualizarTomo(_tomosManager.TomoActual);
    }

    public override void _Process(double delta)
    {
        if (!Visible || _slots.Count == 0) return;

        ActualizarResaltado();
    }

    public void AbrirRueda()
    {
        GD.Print("Abierto");
        // Centra el contenedor en la posición del mouse al abrir
        _contenedor.Position = GetLocalMousePosition() - _contenedor.Size / 2f;

        Visible = true;
    }

    public void CerrarYConfirmar()
    {
        if (!Visible) return;

        if (_indiceResaltado >= 0 && _indiceResaltado < _slots.Count)
        {
            Hechizo elegido = _slots[_indiceResaltado].Hechizo;
            AlSeleccionarHechizo(elegido);
        }

        _indiceResaltado = -1;
        Visible = false;
    }

    private void ActualizarResaltado()
    {
        Vector2 centro = _contenedor.GlobalPosition + _contenedor.Size / 2f;
        Vector2 dirMouse = GetGlobalMousePosition() - centro;

        // Si el mouse está muy cerca del centro, no hay selección (zona muerta)
        if (dirMouse.Length() < 20f)
        {
            SetIndiceResaltado(-1);
            return;
        }

        float anguloMouse = Mathf.RadToDeg(dirMouse.Angle());
        int total = _slots.Count;
        float paso = 360f / total;

        int mejorIndice = 0;
        float mejorDiferencia = float.MaxValue;

        for (int i = 0; i < total; i++)
        {
            float anguloSlot = _anguloInicial + paso * i;
            float diferencia = Mathf.Abs(Mathf.Wrap(anguloMouse - anguloSlot, -180f, 180f));

            if (diferencia < mejorDiferencia)
            {
                mejorDiferencia = diferencia;
                mejorIndice = i;
            }
        }

        SetIndiceResaltado(mejorIndice);
    }

    private void SetIndiceResaltado(int indice)
    {
        if (_indiceResaltado == indice) return;

        if (_indiceResaltado >= 0 && _indiceResaltado < _slots.Count)
            _slots[_indiceResaltado].SetResaltado(false);

        _indiceResaltado = indice;

        if (_indiceResaltado >= 0 && _indiceResaltado < _slots.Count)
            _slots[_indiceResaltado].SetResaltado(true);
    }

    private void ActualizarTomo(Tomo tomo)
    {
        _tomoActual = tomo;
        LimpiarSlots();
        CrearSlots();
    }

    private void LimpiarSlots()
    {
        foreach (Node hijo in _contenedor.GetChildren())
            hijo.QueueFree();

        _slots.Clear();
    }

    private void CrearSlots()
    {
        int total = _tomoActual.Hechizos.Count;
        if (total == 0) return;

        float paso = 360f / total;

        for (int i = 0; i < total; i++)
        {
            Hechizo hechizo = _tomoActual.Hechizos[i];

            SlotHechizo slot = _escenaSlot.Instantiate<SlotHechizo>();
            _contenedor.AddChild(slot);
            slot.AsignarHechizo(hechizo);
            _slots.Add(slot);

            float anguloRad = Mathf.DegToRad(_anguloInicial + paso * i);
            Vector2 offset = new Vector2(Mathf.Cos(anguloRad), Mathf.Sin(anguloRad)) * _radio;

            Vector2 centro = _contenedor.Size / 2f;
            Vector2 tamanoSlot = slot.CustomMinimumSize != Vector2.Zero
                ? slot.CustomMinimumSize
                : new Vector2(64, 64);

            slot.Position = centro + offset - tamanoSlot / 2f;
        }
    }

    private void AlSeleccionarHechizo(Hechizo hechizo)
    {
        GD.Print($"Hechizo confirmado: {hechizo.Nombre}");
        // Acá disparás el cast real
    }
}