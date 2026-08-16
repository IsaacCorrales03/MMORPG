using LiteNetLib;
using Server.Managers;
using Server.Mundo;
using Shared.Paquetes;
using Shared.Tipos;
using Shared.Utils;

namespace Server.Handlers
{
    
    public class AparecerJugadorHandler: IPacketHandler
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
                return;
            }
            _world.AddPlayer(jugadorId, peticion.Position, session.NombreUsuario);
            respuesta.Exitoso = true;
            respuesta.Posicion = new Vector2(0,0);
            //acá podríamos obtener su ultima posicion del session manager
            // luego añadirlo, y luego enviar una respuesta "Aparece" para
            // que el juego pueda saber donde aparecerlo 
            PacketSender.EnviarOrdenado(peer, respuesta);
        }
    }
}