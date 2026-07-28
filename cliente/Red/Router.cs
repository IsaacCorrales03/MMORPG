using System.Text;
using System.Text.Json;
using LiteNetLib;
using Shared.Paquetes;
using Godot;
using Client.Handlers;
using LiteNetLib.Utils;

namespace Client.Red
{
	public class Router
	{
		public static void Enrutar(TipoPaquete tipoPaquete, byte[] contenido, NetPeer peer)
		{
			switch (tipoPaquete)
			{
				case TipoPaquete.RespuestaRegistro:
					{
						var respuesta = JsonSerializer.Deserialize<PaqueteRespuestaRegistro>(contenido);
						if (respuesta is null)
						{
							GD.Print("Contenido inválido en RespuestaRegistro");
							break;
						}
						RegisterHandler.Manejar(respuesta);
						break;
					}
				case TipoPaquete.RespuestaReanudarSesion:
					{
						var respuesta = JsonSerializer.Deserialize<PaqueteRespuestaReanudarSesion>(contenido);
						if (respuesta is null)
						{
							GD.Print("Contenido invalido en RespuestaReanudarSesion");
							break;
						}
						//
						break;
					}
				default:
					GD.Print($"TipoPaquete no manejado ({tipoPaquete}) recibido del servidor");
					break;
			}
		}

		public static void EnviarPaquete(TipoPaquete tipoPaquete, IPaquete paquete)
		{
			NetPeer peer = Conexion.Instance.Peer;

			byte[] contenido = JsonSerializer.SerializeToUtf8Bytes(paquete, paquete.GetType());

			NetDataWriter writer = new NetDataWriter();
			writer.Put((byte)tipoPaquete);
			writer.PutBytesWithLength(contenido);

			peer.Send(writer, DeliveryMethod.ReliableOrdered);
		}
	}
}
