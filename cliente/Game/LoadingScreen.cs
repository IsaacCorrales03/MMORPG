using Client.Juego;
using Godot;
using System;

public partial class LoadingScreen : CanvasLayer
{
	private bool yaImprimio = false;
	[Export]
	public PackedScene LoginScene;
	[Export]
	public PackedScene MainScene;
	public TextureProgressBar BarraProgeso;
	public AnimationPlayer Animaciones;
	public PackedScene siguienteEscena;
	public Timer TimeoutTimer;
	public Control PopUp;
	public Button Reintentar;

	public override void _Ready()
	{
		BarraProgeso = GetNode<TextureProgressBar>("BarraCarga");
		Animaciones = GetNode<AnimationPlayer>("Animaciones");
		TimeoutTimer = GetNode<Timer>("TimeOut");
		PopUp = GetNode<Control>("PopUp");
		Reintentar = GetNode<Button>("PopUp/Reintentar");

		Reintentar.Pressed += OnReintentarPressed;
		TimeoutTimer.Timeout += OnTimeOut;
		GameState.Instance.SesionReanudadaExitosa += OnSesionReanudada;
		GameState.Instance.SesionReanudadaFallida += OnSesionReanudadaFallida;
	}
	public void OnReintentarPressed()
	{
		Conexion.Instance.Connect();
		PopUp.Visible = false;
		TimeoutTimer.Start();
	}

	public void OnTimeOut()
		{
			PopUp.Visible = true;
		}
	private void ChangeScene(StringName animName)
		{
			if (animName != "Cargar")
			{
				return;
			}
			
			Animaciones.AnimationFinished -= ChangeScene;
			GetTree().ChangeSceneToPacked(siguienteEscena);
		}
		public async void OnSesionReanudada()
		{
			GD.Print("Sesión reanudada");
			if (MainScene == null)
			{
				GD.PrintErr("No se asignó la escena siguiente en el inspector.");
				return;
			}
			siguienteEscena = MainScene;
			Animaciones.AnimationFinished += ChangeScene;

			Animaciones.Play("Cargar");
			
		}	
	public void OnSesionReanudadaFallida(string mensaje)
	{
		GD.Print($"Sesión no reanudada: {mensaje}");
		if (LoginScene == null)
		{
			GD.PrintErr("No se asignó la escena siguiente tras fallar en el inspector.");
			return;
		}

		GetTree().ChangeSceneToPacked(LoginScene);
		
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!yaImprimio && GameState.Conectado)
		{
			yaImprimio = true;
			GD.Print("ya se conectó");
		}
	}
}
