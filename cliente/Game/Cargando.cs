using Godot;

public partial class Cargando : CanvasLayer
{
	[Export] private NodePath _rootPath;
	[Export] private NodePath _statusLabelPath;
	[Export] private NodePath _spinnerIconPath;
	[Export] private NodePath _tipLabelPath;

	[Export] private NodePath _errorPopupPath;
	[Export] private NodePath _errorLabelPath;
	[Export] private NodePath _btnReintentarPath;
	[Export] private NodePath _btnCancelarPath;

	private Control _root;
	private Label _statusLabel;
	private TextureRect _spinnerIcon;
	private Label _tipLabel;

	private Panel _errorPopup;
	private Label _errorLabel;
	private Button _btnReintentar;
	private Button _btnCancelar;

	private Tween _spinnerTween;
	private Timer _tipTimer;

	private readonly string[] _tips = new[]
	{
		"Astera se sostiene sobre cinco reinos que dejaron de hablarse hace tres generaciones.",
		"Verdia fue el primer reino en cerrar sus fronteras. Nadie recuerda por qué.",
		"No todos los magos de Astera nacen sabiendo que lo son.",
	};
	private int _tipIndex = 0;

	public override void _Ready()
	{
		_root = GetNode<Control>(_rootPath);
		_statusLabel = GetNode<Label>(_statusLabelPath);
		_spinnerIcon = GetNode<TextureRect>(_spinnerIconPath);
		_tipLabel = GetNode<Label>(_tipLabelPath);

		_errorPopup = GetNode<Panel>(_errorPopupPath);
		_errorLabel = GetNode<Label>(_errorLabelPath);
		_btnReintentar = GetNode<Button>(_btnReintentarPath);
		_btnCancelar = GetNode<Button>(_btnCancelarPath);

		_errorPopup.Visible = false;
		_btnReintentar.Pressed += Reintentar;
		_btnCancelar.Pressed += () => CambiarEscena("res://Escenas/Login.tscn");

		FadeIn();
		StartSpinner();
		StartTipRotation();

		_statusLabel.Text = "Conectando al servidor...";

		Conexion.Instance.OnEstadoCambiado += ManejarCambioDeEstado;
		Conexion.Instance.Connect();
	}

	public override void _ExitTree()
	{
		if (Conexion.Instance != null)
			Conexion.Instance.OnEstadoCambiado -= ManejarCambioDeEstado;

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

	private void StartSpinner()
	{
		_spinnerTween = CreateTween().SetLoops();
		_spinnerTween.TweenProperty(_spinnerIcon, "rotation", Mathf.Tau, 0.9f)
					  .SetTrans(Tween.TransitionType.Linear);
	}

	private void StartTipRotation()
	{
		_tipTimer = new Timer { WaitTime = 4.5, Autostart = true };
		AddChild(_tipTimer);
		_tipTimer.Timeout += RotarTip;
		_tipLabel.Text = _tips[0];
	}

	private void RotarTip()
	{
		_tipIndex = (_tipIndex + 1) % _tips.Length;
		var tween = CreateTween();
		tween.TweenProperty(_tipLabel, "modulate:a", 0f, 0.15f);
		tween.TweenCallback(Callable.From(() => _tipLabel.Text = _tips[_tipIndex]));
		tween.TweenProperty(_tipLabel, "modulate:a", 1f, 0.15f);
	}

	private void ManejarCambioDeEstado(Conexion.EtapaConexion etapa, string mensaje)
	{
		switch (etapa)
		{
			case Conexion.EtapaConexion.Conectando:
				_statusLabel.Text = "Conectando al servidor...";
				break;
			case Conexion.EtapaConexion.Autenticando:
				_statusLabel.Text = "Autenticando...";
				break;
			case Conexion.EtapaConexion.Listo:
				_statusLabel.Text = "Listo";
				CambiarEscena("res://Escenas/MenuPrincipal.tscn");
				break;
			case Conexion.EtapaConexion.Fallido:
				MostrarError(mensaje ?? "No se pudo establecer conexión con el servidor.");
				break;
		}
	}

	private void MostrarError(string mensaje)
	{
		_spinnerTween.Pause();
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
