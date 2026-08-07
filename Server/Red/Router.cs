using LiteNetLib;
using MessagePack;
using Server.Handlers;
using Shared.Paquetes;

namespace Server.Red
{
    public class PacketRouter
    {
        public static readonly Dictionary<TipoPaquete, Type> TiposDePaquete = new() {
            { TipoPaquete.PeticionInicioSesion, typeof(PaquetePeticionInicioSesion) },
            { TipoPaquete.RespuestaInicioSesion, typeof(PaqueteRespuestaInicioSesion) },
            { TipoPaquete.PeticionRegistro, typeof(PaquetePeticionRegistro) },
            { TipoPaquete.RespuestaRegistro, typeof(PaqueteRespuestaRegistro)},
            { TipoPaquete.PeticionReanudarSesion, typeof(PaquetePeticionReanudarSesion)},
            { TipoPaquete.RespuestaReanudarSesion, typeof(PaqueteRespuestaReanudarSesion) }

        };
        public static readonly Dictionary<TipoPaquete, IPacketHandler> PacketHandlers = new()
        {
            {TipoPaquete.PeticionInicioSesion, new LoginHandler()},
            {TipoPaquete.PeticionRegistro, new RegisterHandler()}
        };

        public static async Task Enrutar(TipoPaquete tipo, byte[] contenido, NetPeer peer)
        {
            if (!TiposDePaquete.TryGetValue(tipo, out Type? clasePaquete))
            {
                Console.WriteLine($"Tipo de paquete no registrado: {tipo}");
                return;
            }

            object? resultado;
            try
            {
                resultado = MessagePackSerializer.Deserialize(clasePaquete, contenido);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializando paquete {tipo}: {ex.Message}");
                return;
            }

            if (resultado is not IPaquete paquete)
            {
                Console.WriteLine($"No existe un tipo registrado para {tipo}.");
                return;
            }

            if (!PacketHandlers.TryGetValue(tipo, out IPacketHandler? handler))
            {
                Console.WriteLine($"No existe un handler registrado para {tipo}.");
                return;
            }

            await handler.Handle(peer, paquete);
        }
    }
}