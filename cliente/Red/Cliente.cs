using System;
using Client.Juego;
using Client.Red;
using Godot;
using LiteNetLib;
using Shared.Paquetes;
using Shared.Utils;

public partial class Cliente : Node
{
	private string IP_Servidor = "127.0.0.1";
	private int Puerto = 8455;
	private EventBasedNetListener _listener;
	private NetManager _server;
	private string Token;
	public NetPeer Peer { get; private set; }

	public static Cliente Instancia { get; private set; }


	public enum EstadoConexion
	{
		Desconectado,
		Conectando,
		Conectado,
		Fallida
	}
	public enum EstadoAutenticacion
	{
		NoAutenticado,
		Autenticando,
		Autenticado
	}
	private EstadoConexion estadoConexion;
	private EstadoAutenticacion estadoAutenticacion;
	private Timer _timeoutConexion;
	private const float SegundosTimeout = 8f;

	public event Action<EstadoConexion> OnEstadoConexionCambiado;
	public event Action<EstadoAutenticacion> OnEstadoAutenticacionCambiado;

	public override void _Ready()
	{
		Instancia = this;
		Conectar();
	}
	public override void _Process(double delta)
	{
		_server?.PollEvents();
	}
	public override void _ExitTree()
	{
		_server?.Stop();
	}
	private void CambiarEstadoConexion(EstadoConexion estado)
	{
		estadoConexion = estado;
		GD.Print($"Estado de conexion2: {estadoConexion}");
		OnEstadoConexionCambiado?.Invoke(estado);
	}
	private void CambiarEstadoAutenticacion(EstadoAutenticacion estado)
	{
		estadoAutenticacion = estado;
		GD.Print($"Estado: {estadoAutenticacion}");
		OnEstadoAutenticacionCambiado?.Invoke(estado);
	}

	public void Conectar()
	{
		if (GameState.Conectado || estadoConexion == EstadoConexion.Conectado)
			return;
		Token = TokenManager.LeerTokenGuardado();

		CambiarEstadoConexion(EstadoConexion.Conectando);
		IniciarTimeoutDeConexion();
		_listener = new();
		_server = new(_listener);

		_listener.PeerConnectedEvent += OnPeerConnected;
		_listener.PeerDisconnectedEvent += OnPeerDisconnected;
		_listener.NetworkReceiveEvent += OnNetworkReceive;
		if (!_server.Start())
		{
			CambiarEstadoConexion(EstadoConexion.Desconectado);
			GD.PrintErr("No se pudo iniciar el cliente.");
			return;
		}
		_server.Connect(IP_Servidor, Puerto, Claves.ClaveServidor);
	}
	public void OnNetworkReceive(NetPeer peer, NetPacketReader dataReader, byte channel, DeliveryMethod deliveryMethod)
	{
		try
		{
			GD.Print($"Bytes recibidos: {dataReader.AvailableBytes}");
			TipoPaquete tipo = (TipoPaquete)dataReader.GetByte();
			byte[] contenido = dataReader.GetBytesWithLength();
			Router.Enrutar(tipo, contenido, peer);
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Error inesperado procesando paquete del servidor: {ex}");
		}
		finally
		{
			dataReader.Recycle();
		}
	}

	public void OnPeerConnected(NetPeer conexion)
	{
		CambiarEstadoConexion(EstadoConexion.Conectado);
		GameState.Conectado = true;
		Peer = conexion;
		DetenerTimeout();
		if (!string.IsNullOrEmpty(Token))
		{
			GD.Print($"Token leído del archivo: {Token}");

			PaquetePeticionReanudarSesion paquete = new();
			paquete.Token = Token;

			PacketSender.EnviarTCP(Peer, paquete);
		}
		else
		{
			CambiarEstadoAutenticacion(EstadoAutenticacion.NoAutenticado);
			GD.Print("No se pudo autenticar, no hay token");
		}
	}

	public void OnPeerDisconnected(NetPeer conexion, DisconnectInfo info)
	{
		CambiarEstadoConexion(EstadoConexion.Desconectado);
		GameState.Conectado = false;
		Peer = null;
		DetenerTimeout();
		GD.Print($"{conexion} se desconectó: {info.Reason}");
	}

	private void IniciarTimeoutDeConexion()
	{
		DetenerTimeout();

		_timeoutConexion = new Timer
		{
			WaitTime = SegundosTimeout,
			OneShot = true
		};

		AddChild(_timeoutConexion);

		_timeoutConexion.Timeout += () =>
		{
			CambiarEstadoConexion(EstadoConexion.Fallida);
			_server?.Stop();
			DetenerTimeout();
			GD.PrintErr("Timeout: no se pudo conectar al servidor");
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
