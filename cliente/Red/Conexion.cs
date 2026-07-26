using System.Text;
using System.Text.Json;
using Godot;
using LiteNetLib;
using Shared.Paquetes;
using Shared.Utils;
public partial class Conexion : Node
{
	private string Ip = "127.0.0.1";
	private int Puerto = 8455;
	EventBasedNetListener Listener = new EventBasedNetListener();
	private NetManager Server; 
	private NetPeer Peer; 
	public static Conexion Instance { get; private set; }
	public override void _Ready()
	{
		Instance = this;
		Server = new NetManager(Listener); // primero se crea
		Server.Start();
		Listener.PeerConnectedEvent += (conexion) => { 
			GD.Print("Conectado al server correctamente"); 
			Peer = conexion;
		};
		Server.Connect(Ip, Puerto, Claves.ClaveServidor); // después se usa
	}

	public override void _Process(double delta)
	{
		Server.PollEvents();
	}

	public void  EnviarPaquete(TipoPaquete tipoPaquete, IPaquete paquete) 
	{
		Sobre sobre = new Sobre();
		sobre.TipoDePaquete = tipoPaquete;
		string paquete_json = JsonSerializer.Serialize(paquete, paquete.GetType());
		sobre.Contenido = paquete_json;
		string sobre_json = JsonSerializer.Serialize(sobre);
		byte[] datos = Encoding.UTF8.GetBytes(sobre_json);
		Peer.Send(datos, DeliveryMethod.ReliableOrdered);
	}
}
