using System.Text;
using System.Text.Json;
using LiteNetLib;
using Server.Red.Handlers;
using Shared.Paquetes;

namespace Server.Red
{
    public class Router
    {
        public static async Task Enrutar(Sobre sobre, NetPeer peer)
        {
            switch (sobre.TipoDePaquete)
            {
                case TipoPaquete.PeticionInicioSesion:
                    {
                        var peticion = JsonSerializer.Deserialize<PaquetePeticionInicioSesion>(sobre.Contenido);
                        if (peticion is null)
                        {
                            Console.WriteLine($"Contenido inválido en PeticionInicioSesion de {peer}");
                            break;
                        }
                        break;
                    }
                case TipoPaquete.PeticionRegistro:
                    {
                        var peticion = JsonSerializer.Deserialize<PaquetePeticionRegistro>(sobre.Contenido);
                        if (peticion is null)
                        {
                            Console.WriteLine($"Contenido inválido en PaqueteRespuestaRegistro de {peer}");
                            break;
                        }
                        PaqueteRespuestaRegistro respuesta = await RegisterHandler.Manejar(peticion);
                        EnviarPaquete(TipoPaquete.RespuestaRegistro, respuesta, peer);
                        break;
                    }
            }
        }

        public static void EnviarPaquete(TipoPaquete tipoPaquete, IPaquete paquete, NetPeer peer) 
        {
            Sobre sobre = new Sobre();
            sobre.TipoDePaquete = tipoPaquete;
            string paquete_json = JsonSerializer.Serialize(paquete, paquete.GetType());
            sobre.Contenido = paquete_json;
            string sobre_json = JsonSerializer.Serialize(sobre);
            byte[] datos = Encoding.UTF8.GetBytes(sobre_json);
            peer.Send(datos, DeliveryMethod.ReliableOrdered);
        }
    }
}