using Client.Juego;
using Client.Red;
using Godot;
using Shared.Paquetes;

public partial class Register : Control
{
	private static readonly Color AcentoAmbar = new Color("#C97A3D");

	[Export] private Button _botonRegistrar;
	[Export] private Panel _panel;
	[Export] private LineEdit _userInput;
	[Export] private LineEdit _passwordInput;
	[Export] private LineEdit _emailInput;
	[Export] private Label _errorLabel;
	[Export] private Button _buttonLogin;
	private const string RutaLogin = "res://Escenas/Login.tscn";
	[Export] private PackedScene _escenaMain;

	public override void _Ready()
	{

		_botonRegistrar.Pressed += alPresionarBoton;
		GameState.Instance.RegistroExitoso += OnRegistroExitoso;
		GameState.Instance.RegistroFallido += OnRegistroFallido;
		_buttonLogin.Pressed += alPresionarLogin;

		AgregarEsquinasOrnamentales();
	}

	private void OnRegistroExitoso()
	{

		GameState.Instance.IniciarJuego();
	}

	private void alPresionarLogin()
	{
		GetTree().ChangeSceneToFile(RutaLogin);
	}
	public override void _ExitTree()
	{
		GameState.Instance.RegistroExitoso -= OnRegistroExitoso;
	}

	private void OnRegistroFallido(string mensaje)
	{
		_errorLabel.Text = mensaje;
	}

	private void alPresionarBoton()
	{
		string usuario = _userInput.Text;
		string clave = _passwordInput.Text;
		string email = _emailInput.Text;
		PaquetePeticionRegistro paquete = new()
		{
			Usuario = usuario,
			Clave = clave,
			Email = email,
		};
		Shared.Utils.PacketSender.EnviarTCP(Cliente.Instancia.Peer, paquete);
	}

	private void AgregarEsquinasOrnamentales()
	{
		var ornamentos = new EsquinasOrnamentalesRegister();
		ornamentos.Color = AcentoAmbar;
		ornamentos.SetAnchorsPreset(LayoutPreset.FullRect);
		ornamentos.MouseFilter = Control.MouseFilterEnum.Ignore;
		_panel.AddChild(ornamentos);
	}
}

public partial class EsquinasOrnamentalesRegister : Control
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