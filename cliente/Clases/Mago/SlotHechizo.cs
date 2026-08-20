using Godot;
using Shared.Magia;
using System;

public partial class SlotHechizo : Control
{
    [Export] public Panel _panel;
    [Export] public Label _nombre;
    public bool Seleccionado { get; private set; }
    public Hechizo Hechizo { get; private set; }

    public override void _Ready()
    {
        PivotOffset = Size / 2;

        _nombre.SetAnchorsPreset(LayoutPreset.TopLeft);
        _nombre.Position = Vector2.Zero;
        _nombre.Size = Size;
        _nombre.HorizontalAlignment = HorizontalAlignment.Center;
        _nombre.VerticalAlignment = VerticalAlignment.Center;
    }

    public void AsignarHechizo(Hechizo hechizo)
    {
        _nombre.Text = hechizo.Nombre;
        Hechizo = hechizo;
    }
    public void SetNull()
    {
        Hechizo = null;
        _nombre.Text = "¿?";
    }

    public void SetResaltado(bool resaltado)
    {
        if (Hechizo == null)
            return;
        Modulate = resaltado ? new Color(1.2f, 1.2f, 0.6f) : Colors.White;
        Scale = resaltado ? new Vector2(1.15f, 1.15f) : Vector2.One;
    }
    public void SetSeleccionado(bool valor)
    {
        if (Hechizo == null)
            return;
        Seleccionado = valor;
        // acá luego metemos el highlight 
    }
}