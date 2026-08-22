using LiteNetLib;
using Server.Managers;
using Server.Mundo;
using Shared.Clases;
using Shared.Paquetes;
using Shared.Tipos;
using Shared.Utils;

namespace Server.Handlers
{

    public class AparecerJugadorHandler : IPacketHandler
    {
        private readonly World _world;

        public AparecerJugadorHandler(World world)
        {
            _world = world;
        }
        public async Task Handle(NetPeer peer, IPaquete paquete)
        {
            if (paquete is not PaquetePeticionAparecerJugador peticion)
            {
                return;
            }
            int jugadorId = peticion.JugadorId;
            Sesion? session = SesionManager.ObtenerPorPeer(peer);
            PaqueteRespuestaAparecerJugador respuesta = new();
            if (session == null)
            {
                respuesta.Exitoso = false;
                respuesta.MensajeDeError = "No existe una sesión asociada.";
                ServerLog.Log($"⚠ AparecerJugador rechazado: sin sesión asociada (peer {peer.Id})");
                PacketSender.EnviarOrdenado(peer, respuesta); // faltaba enviar en este caso también
                return;
            }
            Clase clase = CatalogoClases.Crear(session.ClaseId);
            _world.AddPlayer(jugadorId, peticion.Position, session.NombreUsuario, clase);
            respuesta.Exitoso = true;
            respuesta.Posicion = new Vector2(0, 0);
            ServerLog.Log($"✓ Jugador '{session.NombreUsuario}' (id {jugadorId}) apareció en el mundo");
            PacketSender.EnviarOrdenado(peer, respuesta);
        }
    }
}