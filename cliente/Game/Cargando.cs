using Godot;
using LiteNetLib;

public partial class Cargando : CanvasLayer
{
	[Export] private Control _root;
	[Export] private Label _statusLabel;
	[Export] private TextureRect _spinnerIcon;
	[Export] private Label _tipLabel;
	[Export] private Panel _errorPopup;
	[Export] private Label _errorLabel;
	[Export] private Button _btnReintentar;
	[Export] private Button _btnCancelar;
	[Export] private Label _serverInfo;
	[Export] private ProgressBar _progressBar;
	
	private Tween _spinnerTween;
	private Timer _tipTimer;
	private Timer _pingTimer;
	private int _tipIndex = 0;

	private readonly string[] _tips = new[]
	{
		"Azura es el continente más estable de todo Astera",
		"Hablar con los sabios podría potenciar tus hechizos...",
		"Cada espadazo que aciertes aumentará tu técnica con la espada",
		"No se trata de lo que logras, si no de con quien lo logras..."
	};

	public override void _Ready()
	{

		_errorPopup.Visible = false;
		_btnReintentar.Pressed += Reintentar;
		_btnCancelar.Pressed += () => CambiarEscena("res://Escenas/Login.tscn");
		
		FadeIn();
		StartTipRotation();

		IniciarActualizacionPing();
		Cliente.Instancia.OnEstadoConexionCambiado += ManejarCambioDeEstadoConexion;
		Cliente.Instancia.Conectar();
	}

	

	public override void _ExitTree()
	{
		if (Conexion.Instance != null)
			Cliente.Instancia.OnEstadoConexionCambiado -= ManejarCambioDeEstadoConexion;
		if (_tipTimer != null)
			_tipTimer.Timeout -= RotarTip;
		if (_btnReintentar != null)
			_btnReintentar.Pressed -= Reintentar;
	}

	private void FadeIn()
	{
		_root.Modulate = new Color(1, 1, 1, 0);
		var tween = CreateTween();
		tween.TweenProperty(_root, "modulate:a", 1.0f, 0.35f)
			 .SetEase(Tween.EaseType.Out);
	}


	private void StartTipRotation()
	{
		_tipTimer = new Timer { WaitTime = 4.5, Autostart = true };
		AddChild(_tipTimer);
		_tipTimer.Timeout += RotarTip;
		_tipLabel.Text = _tips[0];
	}
	private void IniciarActualizacionPing()
	{
		_pingTimer = new Timer
		{
			WaitTime = 1.0,
			OneShot = false
		};

		AddChild(_pingTimer);

		_pingTimer.Timeout += ActualizarPing;

		_pingTimer.Start();
	}

	private void ActualizarPing()
	{
		if (Cliente.Instancia.Peer == null || Cliente.Instancia.Peer.ConnectionState != ConnectionState.Connected)
			return;

		GD.Print($"Ping: {Cliente.Instancia.Peer.Ping} ms");
		_serverInfo.Text = $"Ping: {Cliente.Instancia.Peer.Ping} ms";
	}
	private void RotarTip()
	{
		_tipIndex = (_tipIndex + 1) % _tips.Length;
		var tween = CreateTween();
		tween.TweenProperty(_tipLabel, "modulate:a", 0f, 0.15f);
		tween.TweenCallback(Callable.From(() => _tipLabel.Text = _tips[_tipIndex]));
		tween.TweenProperty(_tipLabel, "modulate:a", 1f, 0.15f);
	}

	private void ManejarCambioDeEstadoConexion(Cliente.EstadoConexion estado)
	{
		switch (estado)
		{
			case Cliente.EstadoConexion.Conectando:
				_statusLabel.Text = "Conectando al servidor...";
				break;
			case Cliente.EstadoConexion.Conectado:
				_statusLabel.Text = "Conectado correctamente";
				break;
			case Cliente.EstadoConexion.Fallida:
				_statusLabel.Text = "Desconectado";
				MostrarError("No se pudo establecer conexión con el servidor.");
				break;
			case Cliente.EstadoConexion.Desconectado:
				_statusLabel.Text = "No se pudo establecer la conexión con el servidor";
				MostrarError("No se pudo establecer conexión con el servidor.");
				break;
			default:
				break;
		}
	}

	private void ManejarCambioDeEstadoAutenticacion(Cliente.EstadoAutenticacion estado)
	{
		switch (estado)
		{
			case Cliente.EstadoAutenticacion.Autenticando:
				_statusLabel.Text = "Autenticando sesión";
				break;
			case Cliente.EstadoAutenticacion.NoAutenticado:
				_statusLabel.Text = "No se pudo autenticar";
				break;
			case Cliente.EstadoAutenticacion.Autenticado:
				_statusLabel.Text = "Sesion autenticada";
				break;
			default:
				break;
		}
	}

	private void MostrarError(string mensaje)
	{
		_errorLabel.Text = mensaje;
		_errorPopup.Visible = true;
		_errorPopup.Modulate = new Color(1, 1, 1, 0);
		var tween = CreateTween();
		tween.TweenProperty(_errorPopup, "modulate:a", 1f, 0.2f);
	}

	private void Reintentar()
	{
		_errorPopup.Visible = false;
		_spinnerTween.Play();
		_statusLabel.Text = "Conectando al servidor...";
		Conexion.Instance.Connect();
	}

	private void CambiarEscena(string ruta)
	{
		var tween = CreateTween();
		tween.TweenProperty(_root, "modulate:a", 0f, 0.4f)
			 .SetEase(Tween.EaseType.In);
		tween.TweenCallback(Callable.From(() =>
			GetTree().ChangeSceneToFile(ruta)));
	}
}
