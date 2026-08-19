using Godot;
using Shared.Magia;
using System;

public partial class SlotHechizo : Control
{
    [Export] private Button _boton;

    public Hechizo Hechizo { get; private set; }

    public void AsignarHechizo(Hechizo hechizo)
    {
        Hechizo = hechizo;
        _boton.Text = hechizo.Nombre;
        _boton.MouseFilter = MouseFilterEnum.Ignore; // el botón ya no procesa click propio
    }

    public void SetResaltado(bool resaltado)
    {
        Modulate = resaltado ? new Color(1.2f, 1.2f, 0.6f) : Colors.White;
        Scale = resaltado ? new Vector2(1.15f, 1.15f) : Vector2.One;
    }
}