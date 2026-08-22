using LiteNetLib;
using MessagePack;
using Server.Handlers;
using Server.Managers;
using Server.Mundo;
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
            { TipoPaquete.RespuestaReanudarSesion, typeof(PaqueteRespuestaReanudarSesion) },
            { TipoPaquete.PeticionAparecerJugador, typeof(PaquetePeticionAparecerJugador)},
            { TipoPaquete.Movimiento, typeof(PaqueteMovimiento)}
        };

        private readonly World _world;
        private readonly Dictionary<TipoPaquete, IPacketHandler> _packetHandlers;

        public PacketRouter(World world)
        {
            _world = world;
            _packetHandlers = new()
            {
                { TipoPaquete.PeticionInicioSesion, new LoginHandler() },
                { TipoPaquete.PeticionRegistro, new RegisterHandler() },
                { TipoPaquete.PeticionReanudarSesion, new ResumeSessionHandler() },
                { TipoPaquete.PeticionAparecerJugador, new AparecerJugadorHandler(_world) }
            };
        }


        public async Task Enrutar(TipoPaquete tipo, byte[] contenido, NetPeer peer)
        {
            if (!TiposDePaquete.TryGetValue(tipo, out Type? clasePaquete))
            {
                ServerLog.Log($"Tipo de paquete no registrado: {tipo}");
                return;
            }

            object? resultado;
            try
            {
                resultado = MessagePackSerializer.Deserialize(clasePaquete, contenido);
            }
            catch (Exception ex)
            {
                ServerLog.Log($"Error deserializando paquete {tipo}: {ex.Message}");
                return;
            }

            if (resultado is not IPaquete paquete)
            {
                ServerLog.Log($"No existe un tipo registrado para {tipo}.");
                return;
            }

            if (!_packetHandlers.TryGetValue(tipo, out IPacketHandler? handler))
            {
                if (!_world.ManejaEvento(tipo))
                {
                    ServerLog.Log(
                        $"No existe handler ni evento para {tipo}."
                    );
                    return;
                }

                int? jugadorId = SesionManager
                    .ObtenerPorPeer(peer)?
                    .UsuarioId;

                if (jugadorId is not int id)
                {
                    ServerLog.Log(
                        $"No se pudo identificar al jugador para {tipo}."
                    );
                    return;
                }

                if (!_world.players.ContainsKey(id))
                {
                    ServerLog.Log(
                        $"El jugador {id} no existe en World."
                    );
                    return;
                }

                _world.AddEvent(paquete, id);
                return;
            }
            await handler.Handle(peer, paquete);
        }
    }
}