using Godot;
using Shared.Magia;
using System;
using System.Collections.Generic;

public partial class SlotHechizo : Control
{
    [Export] public Panel _panel;
    [Export] public Label _nombre;
    [Export] public TextureRect _icono;
    [Export] public Panel _franjaNombre;

    public bool Seleccionado { get; private set; }
    public Hechizo Hechizo { get; private set; }

    private const string RutaIconos = "res://Assets/Hechizos/";
    private static readonly Dictionary<string, Texture2D> _cacheTexturas = new();

    public override void _Ready()
    {
        PivotOffset = Size / 2;

        _nombre.SetAnchorsPreset(LayoutPreset.BottomWide);
        _nombre.HorizontalAlignment = HorizontalAlignment.Center;
        _nombre.VerticalAlignment = VerticalAlignment.Center;

        if (_icono != null)
        {
            _icono.SetAnchorsPreset(LayoutPreset.FullRect);
            _icono.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            _icono.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        }
    }

    public void AsignarHechizo(Hechizo hechizo)
    {
        Hechizo = hechizo;
        _nombre.Text = hechizo.Nombre;

        if (_icono != null)
        {
            _icono.Texture = ObtenerTextura(hechizo.IconName);
            _icono.Visible = true;
        }

        if (_franjaNombre != null)
            _franjaNombre.Visible = true;
    }

    public void SetNull()
    {
        Hechizo = null;
        _nombre.Text = "¿?";

        if (_icono != null)
        {
            _icono.Texture = null;
            _icono.Visible = false;
        }

        if (_franjaNombre != null)
            _franjaNombre.Visible = false;
    }

    private static Texture2D ObtenerTextura(string iconName)
    {
        if (string.IsNullOrEmpty(iconName))
            return null;

        if (_cacheTexturas.TryGetValue(iconName, out var cacheada))
            return cacheada;

        var textura = GD.Load<Texture2D>($"{RutaIconos}{iconName}.png");
        if (textura != null)
            _cacheTexturas[iconName] = textura;
        else
            GD.PushWarning($"[SlotHechizo] No se encontró la textura: {RutaIconos}{iconName}.png");

        return textura;
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