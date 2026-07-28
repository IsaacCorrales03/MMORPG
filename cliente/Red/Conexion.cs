using System;
using System.Text.Json;
using Godot;
using LiteNetLib;
using Shared.Paquetes;
using Shared.Utils;
using System.IO;
using System.Security.Cryptography;
using Client.Red;


public partial class Conexion : Node
{
	private string Ip = "127.0.0.1";
	private int Puerto = 8455;
	EventBasedNetListener Listener = new EventBasedNetListener();
	private NetManager Server;
	public NetPeer Peer;
	public static Conexion Instance { get; private set; }
	public override void _Ready()
	{
		Instance = this;
		string tokenGuardado = TokenManager.LeerTokenGuardado();
		
		Server = new NetManager(Listener); // primero se crea
		Server.Start();
		Listener.PeerConnectedEvent += (conexion) =>
		{
			GD.Print("Conectado al server correctamente");
			Peer = conexion;
			if (!string.IsNullOrEmpty(tokenGuardado))
			{
				GD.Print($"Token leído del archivo: {tokenGuardado}");
				PaquetePeticionReanudarSesion paquete = new();
				paquete.Token = tokenGuardado;
				Client.Red.Router.EnviarPaquete(TipoPaquete.PeticionReanudarSesion, paquete);
			}
		};
		Listener.NetworkReceiveEvent += (peer, dataReader, deliveryMethod, channel) =>
		{
			try
			{
				TipoPaquete tipo = (TipoPaquete)dataReader.GetByte();
				byte[] contenido = dataReader.GetBytesWithLength();

				Client.Red.Router.Enrutar(tipo, contenido, peer);
			}
			catch (JsonException ex)
			{
				GD.PrintErr($"Error deserializando paquete del servidor: {ex.Message}");
			}
			catch (Exception ex)
			{
				GD.PrintErr($"Error inesperado procesando paquete del servidor: {ex}");
			}
			finally
			{
				dataReader.Recycle();
			}
		};
		Server.Connect(Ip, Puerto, Claves.ClaveServidor); // después se usa
		
	}

	public override void _Process(double delta)
	{
		Server.PollEvents();
	}

	
}
