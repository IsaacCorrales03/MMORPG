using System.Linq.Expressions;
using System.Text.Json;
using Godot;
using Shared.Paquetes;

public partial class EstiloLoginPanel : Control
{
	// --- Paleta ---
	private static readonly Color FondoGeneral = new Color("#12100E");
	private static readonly Color FondoPanel = new Color("#1B1611");
	private static readonly Color AcentoAmbar = new Color("#C97A3D");
	private static readonly Color DoradoApagado = new Color("#8A6A3B");
	private static readonly Color TextoPrincipal = new Color("#EDE4D3");
	private static readonly Color TextoSecundario = new Color("#8C8478");

	private Button boton;
	private ColorRect fondo; 
	private Panel panel;
	private Label titulo;
	private Label etiquetaUsuario;
	private Label etiquetaClave; 
	private LineEdit userInput;
	private LineEdit passwordInput;
	private LineEdit emailInput;
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
		emailInput = GetNode<LineEdit>("PanelLogin/MarginContainer/VBoxContainer/EmailInput");
		boton.Pressed += alPresionarBoton;
		AplicarFondoGeneral();
		AplicarEstiloPanel();
		AplicarEstiloTitulo();
		AplicarEstiloLabels();
		AplicarEstiloCampos();
		AplicarEstiloBoton();
		AgregarEsquinasOrnamentales();
	}
	private void alPresionarBoton()
	{
		string usuario = userInput.Text;
		string clave = passwordInput.Text;
		string email = emailInput.Text;
		PaquetePeticionRegistro paquete = new();
		paquete.Usuario = usuario;
		paquete.Clave = clave;
		paquete.Email = email;

		Conexion.Instance.EnviarPaquete(TipoPaquete.PeticionRegistro, paquete);
		
	}
	private void AplicarFondoGeneral()
	{
		if (fondo != null)
			fondo.Color = FondoGeneral;
	}

	private void AplicarEstiloPanel()
	{
		var estilo = new StyleBoxFlat();
		estilo.BgColor = FondoPanel;
		estilo.SetCornerRadiusAll(4); // casi recto, look de sello grabado, no "app moderna"
		estilo.SetBorderWidthAll(1);
		estilo.BorderColor = DoradoApagado;
		estilo.ShadowSize = 24;
		estilo.ShadowColor = new Color(0, 0, 0, 0.45f);

		panel.AddThemeStyleboxOverride("panel", estilo);
	}

	private void AplicarEstiloTitulo()
	{
		titulo.Text = "I N I C I A R   S E S I Ó N"; // tracking manual vía espaciado
		titulo.AddThemeColorOverride("font_color", TextoPrincipal);
		titulo.AddThemeFontSizeOverride("font_size", 30);
		titulo.HorizontalAlignment = HorizontalAlignment.Center;
	}

	private void AplicarEstiloLabels()
	{
		foreach (var nombre in new[] { "User", "Password" })
		{
			var label = GetNode<Label>($"PanelLogin/MarginContainer/VBoxContainer/{nombre}");
			label.AddThemeColorOverride("font_color", TextoSecundario);
			label.AddThemeFontSizeOverride("font_size", 15);
		}

		// Ajustamos el texto para que se sientan como inscripciones, no placeholders de formulario
		etiquetaUsuario.Text = "USUARIO";
		etiquetaClave.Text = "CONTRASEÑA";
	}

	private void AplicarEstiloCampos()
	{
		foreach (var nombre in new[] { "UserInput", "PasswordInput" })
		{
			var campo = GetNode<LineEdit>($"PanelLogin/MarginContainer/VBoxContainer/{nombre}");

			// Estado normal: solo línea inferior, sin caja rellena
			var estiloNormal = new StyleBoxFlat();
			estiloNormal.BgColor = new Color(0, 0, 0, 0); // transparente
			estiloNormal.BorderWidthBottom = 1;
			estiloNormal.BorderColor = DoradoApagado;
			estiloNormal.ContentMarginBottom = 10;
			estiloNormal.ContentMarginTop = 6;

			// Estado con foco: línea inferior en color ámbar, más gruesa
			var estiloFoco = (StyleBoxFlat)estiloNormal.Duplicate();
			estiloFoco.BorderWidthBottom = 2;
			estiloFoco.BorderColor = AcentoAmbar;

			campo.AddThemeStyleboxOverride("normal", estiloNormal);
			campo.AddThemeStyleboxOverride("focus", estiloFoco);
			campo.AddThemeColorOverride("font_color", TextoPrincipal);
			campo.AddThemeColorOverride("caret_color", AcentoAmbar);
			campo.AddThemeFontSizeOverride("font_size", 17);
			campo.CustomMinimumSize = new Vector2(0, 53);
		}
	}

	private void AplicarEstiloBoton()
	{
		boton.Text = "E N T R A R";
		boton.CustomMinimumSize = new Vector2(0, 60);

		var estiloNormal = new StyleBoxFlat();
		estiloNormal.BgColor = new Color(0, 0, 0, 0);
		estiloNormal.SetBorderWidthAll(1);
		estiloNormal.BorderColor = AcentoAmbar;
		estiloNormal.SetCornerRadiusAll(2);

		var estiloHover = (StyleBoxFlat)estiloNormal.Duplicate();
		estiloHover.BgColor = new Color(AcentoAmbar, 0.12f);

		var estiloPressed = (StyleBoxFlat)estiloNormal.Duplicate();
		estiloPressed.BgColor = new Color(AcentoAmbar, 0.25f);

		boton.AddThemeStyleboxOverride("normal", estiloNormal);
		boton.AddThemeStyleboxOverride("hover", estiloHover);
		boton.AddThemeStyleboxOverride("pressed", estiloPressed);
		boton.AddThemeColorOverride("font_color", AcentoAmbar);
		boton.AddThemeColorOverride("font_hover_color", TextoPrincipal);
		boton.AddThemeFontSizeOverride("font_size", 18);
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
