using System;
using System.Text.Json;
using Godot;
using LiteNetLib;
using Shared.Paquetes;
using Shared.Utils;
using Client.Red;
using Client.Juego;

public partial class Conexion : Node
{
	public enum EtapaConexion
	{
		Conectando,
		Autenticando,
		Listo,
		Fallido
	}

	public event Action<EtapaConexion, string> OnEstadoCambiado;

	private string Ip = "127.0.0.1";
	private int Puerto = 8455;
	EventBasedNetListener Listener = new EventBasedNetListener();
	private NetManager Server;
	public NetPeer Peer;
	public static Conexion Instance { get; private set; }

	private Timer _timeoutConexion;
	private const float SegundosTimeout = 8f;
	private bool _intentoResuelto; // evita doble notificación
	
	public override void _Ready()
	{
		Connect();
	}

	public void Connect()
	{
		Instance = this;
		string tokenGuardado = TokenManager.LeerTokenGuardado();
		_intentoResuelto = false;
	
		NotificarEstado(EtapaConexion.Conectando, null);
		IniciarTimeoutDeConexion();

		Server = new NetManager(Listener);
		Server.Start();

		Server.Connect(Ip, Puerto, Claves.ClaveServidor);
	}

	public override void _Process(double delta)
	{
		Server?.PollEvents();
	}

	public void NotificarSesionReanudada(bool exito)
	{
		DetenerTimeout();
		if (exito)
			MarcarResueltoYNotificar(EtapaConexion.Listo, null);
		else
			MarcarResueltoYNotificar(EtapaConexion.Fallido, "Tu sesión expiró. Iniciá sesión de nuevo.");
	}

	private void MarcarResueltoYNotificar(EtapaConexion etapa, string mensaje)
	{
		if (_intentoResuelto) return; // ya se resolvió, ignoramos eventos tardíos/duplicados
		_intentoResuelto = true;
		NotificarEstado(etapa, mensaje);
	}

	private void NotificarEstado(EtapaConexion etapa, string mensaje)
	{
		OnEstadoCambiado?.Invoke(etapa, mensaje);
	}

	private void IniciarTimeoutDeConexion()
	{
		_timeoutConexion = new Timer { WaitTime = SegundosTimeout, OneShot = true };
		AddChild(_timeoutConexion);
		_timeoutConexion.Timeout += () =>
		{
			GD.PrintErr("Timeout: no se pudo conectar al servidor");
			MarcarResueltoYNotificar(EtapaConexion.Fallido, "El servidor no respondió a tiempo.");
		};
		_timeoutConexion.Start();
	}

	private void IniciarTimeoutDeAutenticacion()
	{
		_timeoutConexion = new Timer { WaitTime = SegundosTimeout, OneShot = true };
		AddChild(_timeoutConexion);
		_timeoutConexion.Timeout += () =>
		{
			GD.PrintErr("Timeout: el servidor no respondió la reanudación de sesión");
			MarcarResueltoYNotificar(EtapaConexion.Fallido, "No se pudo verificar tu sesión.");
		};
		_timeoutConexion.Start();
	}

	private void DetenerTimeout()
	{
		if (_timeoutConexion != null && IsInstanceValid(_timeoutConexion))
		{
			_timeoutConexion.Stop();
			_timeoutConexion.QueueFree();
			_timeoutConexion = null;
		}
	}
}
