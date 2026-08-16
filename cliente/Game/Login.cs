using System.Linq.Expressions;
using System.Text.Json;
using Client.Juego;
using Client.Red;
using Godot;
using Shared.Paquetes;
using Shared.Utils;

public partial class Login : Control
{
	// --- Paleta ---

	private static readonly Color AcentoAmbar = new Color("#C97A3D");
	private static readonly Color TextoPrincipal = new Color("#EDE4D3");

	private Button boton;
	private ColorRect fondo;
	private Panel panel;
	private Label titulo;
	private Label etiquetaUsuario;
	private Label etiquetaClave;
	private LineEdit userInput;
	private LineEdit passwordInput;

	// --- Overlay de resultado (éxito / fallo) ---
	private Control overlayResultado;
	private Label labelResultado;
	private Button botonReintentar;
	[Export] private Label _errorLabel;
	private const string RutaRegistrar = "res://Escenas/register.tscn";

	[Export] private PackedScene _escenaMain;
	[Export] private Button _buttonRegistrar;

	public override void _Ready()
	{
		boton = GetNode<Button>("PanelLogin/MarginContainer/VBoxContainer/BotonIniciar");
		fondo = GetNodeOrNull<ColorRect>("Fondo");
		panel = GetNode<Panel>("PanelLogin");
		titulo = GetNode<Label>("PanelLogin/MarginContainer/VBoxContainer/titulo");
		etiquetaUsuario = GetNode<Label>("PanelLogin/MarginContainer/VBoxContainer/User");
		etiquetaClave = GetNode<Label>("PanelLogin/MarginContainer/VBoxContainer/Password");
		userInput = GetNode<LineEdit>("PanelLogin/MarginContainer/VBoxContainer/UserInput");
		passwordInput = GetNode<LineEdit>("PanelLogin/MarginContainer/VBoxContainer/PasswordInput");
		boton.Pressed += alPresionarBoton;
		GameState.Instance.InicioSesionExitoso += OnInicicioSesionExitoso;
		GameState.Instance.InicioSesionFallido += OnInicicioSesionFallido;
		_buttonRegistrar.Pressed += alPresionarRegistrar;
		AgregarEsquinasOrnamentales();
		CrearOverlayResultado();
	}
	private void OnInicicioSesionExitoso()
	{
		GameState.Instance.IniciarJuego();
	}
	private void alPresionarRegistrar()
	{
		GetTree().ChangeSceneToFile(RutaRegistrar);
	}

	public override void _ExitTree()
	{
		GameState.Instance.InicioSesionExitoso -= OnInicicioSesionExitoso;
		GameState.Instance.InicioSesionFallido -= OnInicicioSesionFallido;
	}
	private void OnInicicioSesionFallido(string mensaje)
	{
		_errorLabel.Text = mensaje;
	}

	private void alPresionarBoton()
	{
		string usuario = userInput.Text;
		string clave = passwordInput.Text;
		PaquetePeticionInicioSesion paquete = new()
		{
			Usuario = usuario,
			Clave = clave,
		};
		PacketSender.EnviarOrdenado(Cliente.Instancia.Peer, paquete);
	}



	// --- El elemento "firma" del diseño: esquinas ornamentales tipo sello ---
	private void AgregarEsquinasOrnamentales()
	{
		var panel = GetNode<Panel>("PanelLogin");
		var ornamentos = new EsquinasOrnamentales();
		ornamentos.Color = AcentoAmbar;
		ornamentos.SetAnchorsPreset(LayoutPreset.FullRect);
		ornamentos.MouseFilter = Control.MouseFilterEnum.Ignore;
		panel.AddChild(ornamentos);
	}

	// --- Overlay de resultado ---

	private void CrearOverlayResultado()
	{
		overlayResultado = new Control();
		overlayResultado.SetAnchorsPreset(LayoutPreset.FullRect);
		overlayResultado.MouseFilter = Control.MouseFilterEnum.Stop;
		overlayResultado.Visible = false;
		AddChild(overlayResultado);

		var fondoOverlay = new ColorRect();
		fondoOverlay.SetAnchorsPreset(LayoutPreset.FullRect);
		overlayResultado.AddChild(fondoOverlay);

		var centro = new CenterContainer();
		centro.SetAnchorsPreset(LayoutPreset.FullRect);
		overlayResultado.AddChild(centro);

		var caja = new VBoxContainer();
		caja.AddThemeConstantOverride("separation", 28);
		centro.AddChild(caja);

		labelResultado = new Label();
		labelResultado.HorizontalAlignment = HorizontalAlignment.Center;
		labelResultado.AddThemeColorOverride("font_color", TextoPrincipal);
		labelResultado.AddThemeFontSizeOverride("font_size", 26);
		caja.AddChild(labelResultado);

		botonReintentar = new Button();
		botonReintentar.Text = "V O L V E R   A   I N T E N T A R";
		botonReintentar.CustomMinimumSize = new Vector2(280, 60);
		botonReintentar.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
		botonReintentar.Pressed += OnBotonReintentarPresionado;

		var estiloNormal = new StyleBoxFlat();
		estiloNormal.BgColor = new Color(0, 0, 0, 0);
		estiloNormal.SetBorderWidthAll(1);
		estiloNormal.BorderColor = AcentoAmbar;
		estiloNormal.SetCornerRadiusAll(2);

		var estiloHover = (StyleBoxFlat)estiloNormal.Duplicate();
		estiloHover.BgColor = new Color(AcentoAmbar, 0.12f);

		var estiloPressed = (StyleBoxFlat)estiloNormal.Duplicate();
		estiloPressed.BgColor = new Color(AcentoAmbar, 0.25f);

		botonReintentar.AddThemeStyleboxOverride("normal", estiloNormal);
		botonReintentar.AddThemeStyleboxOverride("hover", estiloHover);
		botonReintentar.AddThemeStyleboxOverride("pressed", estiloPressed);
		botonReintentar.AddThemeColorOverride("font_color", AcentoAmbar);
		botonReintentar.AddThemeColorOverride("font_hover_color", TextoPrincipal);
		botonReintentar.AddThemeFontSizeOverride("font_size", 16);

		caja.AddChild(botonReintentar);
	}

	private void MostrarResultado(bool exito, string mensaje)
	{
		labelResultado.Text = mensaje;
		labelResultado.AddThemeColorOverride("font_color", exito ? TextoPrincipal : AcentoAmbar);

		if (fondo != null)
			fondo.Visible = false;
		panel.Visible = false;

		overlayResultado.Visible = true;
	}

	private void OnBotonReintentarPresionado()
	{
		overlayResultado.Visible = false;

		if (fondo != null)
			fondo.Visible = true;
		panel.Visible = true;

		userInput.Text = "";
		passwordInput.Text = "";
	}
}

// Control auxiliar que dibuja 4 marcas en forma de "L" en las esquinas del panel,
// como un sello o marco de manuscrito antiguo.
public partial class EsquinasOrnamentales : Control
{
	public Color Color = new Color("#C97A3D");
	private const float Largo = 22f;
	private const float Grosor = 2f;
	private const float Margen = 10f;

	public override void _Draw()
	{
		var tam = Size;

		DibujarEsquina(new Vector2(Margen, Margen), new Vector2(1, 1));
		DibujarEsquina(new Vector2(tam.X - Margen, Margen), new Vector2(-1, 1));
		DibujarEsquina(new Vector2(Margen, tam.Y - Margen), new Vector2(1, -1));
		DibujarEsquina(new Vector2(tam.X - Margen, tam.Y - Margen), new Vector2(-1, -1));
	}

	private void DibujarEsquina(Vector2 origen, Vector2 direccion)
	{
		DrawLine(origen, origen + new Vector2(Largo * direccion.X, 0), Color, Grosor);
		DrawLine(origen, origen + new Vector2(0, Largo * direccion.Y), Color, Grosor);
	}
}
