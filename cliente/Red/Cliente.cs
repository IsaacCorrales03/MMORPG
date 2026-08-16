using System;
using Client.Juego;
using Client.Red;
using Godot;
using LiteNetLib;
using Shared.Paquetes;
using Shared.Utils;

public partial class Cliente : Node
{
	private string IP_Servidor;
	private int Puerto;
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
	private EstadoConexion estadoConexion = EstadoConexion.Desconectado;
	private EstadoAutenticacion estadoAutenticacion;
	private Timer _timeoutConexion;
	private const float SegundosTimeout = 8f;

	public event Action<EstadoConexion> OnEstadoConexionCambiado;
	public event Action<EstadoAutenticacion> OnEstadoAutenticacionCambiado;

	public override void _Ready()
	{
		Instancia = this;
		CargarConfiguracion();
		IniciarTimeoutDeConexion();
		GameState.Instance.SesionReanudadaFallida += OnSesionReanudadaFallida;
		GameState.Instance.SesionReanudadaExitosa += OnSesionReanudadaExitosa;

	}
	private void CargarConfiguracion()
	{
		string ruta = OS.GetExecutablePath().GetBaseDir().PathJoin("config.json");

		if (!FileAccess.FileExists(ruta))
		{
			string configDefault = """
		{
			"server_ip": "127.0.0.1",
			"server_port": 8455
		}
		""";

			using var archivo = FileAccess.Open(ruta, FileAccess.ModeFlags.Write);
			archivo.StoreString(configDefault);

			GD.Print("Configuración creada en: " + ruta);
		}

		string contenido = FileAccess.GetFileAsString(ruta);

		var json = new Json();

		if (json.Parse(contenido) != Error.Ok)
		{
			GD.PrintErr("Error leyendo config.json");
			return;
		}

		var config = (Godot.Collections.Dictionary)json.Data;

		IP_Servidor = config["server_ip"].ToString();
		Puerto = Convert.ToInt32(config["server_port"]);
	}
	public EstadoConexion get_status()
	{
		return estadoConexion;
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
		GD.Print($"Estado de conexion: {estadoConexion}");
		OnEstadoConexionCambiado?.Invoke(estado);
	}
	private void CambiarEstadoAutenticacion(EstadoAutenticacion estado)
	{
		estadoAutenticacion = estado;
		GD.Print($"Estado sesion: {estadoAutenticacion}");
		OnEstadoAutenticacionCambiado?.Invoke(estado);
	}

	public void Conectar()
	{
		if (GameState.Conectado || estadoConexion == EstadoConexion.Conectado)
			return;

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
	private void OnSesionReanudadaFallida(string mensaje)
	{
		CambiarEstadoAutenticacion(EstadoAutenticacion.NoAutenticado);
	}
	private void OnSesionReanudadaExitosa()
	{
		CambiarEstadoAutenticacion(EstadoAutenticacion.Autenticado);
	}
	public void OnNetworkReceive(NetPeer peer, NetPacketReader dataReader, byte channel, DeliveryMethod deliveryMethod)
	{
		try
		{
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
		Token = TokenManager.LeerTokenGuardado();
		if (!string.IsNullOrEmpty(Token))
		{
			CambiarEstadoAutenticacion(EstadoAutenticacion.Autenticando);

			PaquetePeticionReanudarSesion paquete = new();
			paquete.Token = Token;

			PacketSender.EnviarOrdenado(Peer, paquete);
		}
		else
		{
			CambiarEstadoAutenticacion(EstadoAutenticacion.NoAutenticado);
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
