using System.Text;
using System.Text.Json;
using LiteNetLib;
using LiteNetLib.Utils;
using Server.Handlers;
using Server.Red.Handlers;
using Shared.Paquetes;

namespace Server.Red
{
    public class Router
    {
        public static async Task Enrutar(TipoPaquete tipoPaquete, byte[] contenido, NetPeer peer)
        {
            switch (tipoPaquete)
            {
                case TipoPaquete.PeticionInicioSesion:
                    {
                        var peticion = JsonSerializer.Deserialize<PaquetePeticionInicioSesion>(contenido);
                        if (peticion is null)
                        {
                            Console.WriteLine($"Contenido inválido en PeticionInicioSesion de {peer}");
                            break;
                        }
                        // ... manejar login
                        break;
                    }
                case TipoPaquete.PeticionRegistro:
                    {
                        var peticion = JsonSerializer.Deserialize<PaquetePeticionRegistro>(contenido);
                        if (peticion is null)
                        {
                            Console.WriteLine($"Contenido inválido en PeticionRegistro de {peer}");
                            break;
                        }
                        PaqueteRespuestaRegistro respuesta = await RegisterHandler.Manejar(peticion, peer);
                        EnviarPaquete(TipoPaquete.RespuestaRegistro, respuesta, peer);
                        break;
                    }
                case TipoPaquete.PeticionReanudarSesion:
                    {
                        var peticion = JsonSerializer.Deserialize<PaquetePeticionReanudarSesion>(contenido);
                        if (peticion is null)
                        {
                            Console.WriteLine($"Contenido inválido en PeticionReanudarSesion de {peer}");
                            break;
                        }

                        PaqueteRespuestaReanudarSesion respuesta = await ResumeSessionHandler.Manejar(peticion, peer);
                        EnviarPaquete(TipoPaquete.RespuestaReanudarSesion, respuesta,peer);
                        break;
                    }
                default:
                    Console.WriteLine($"TipoPaquete no manejado ({tipoPaquete}) recibido de {peer}");
                    break;
            }
        }

        public static void EnviarPaquete(TipoPaquete tipoPaquete, IPaquete paquete, NetPeer peer)
        {
            byte[] contenido = JsonSerializer.SerializeToUtf8Bytes(paquete, paquete.GetType());

            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)tipoPaquete);
            writer.PutBytesWithLength(contenido);

            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }
}