using LiteNetLib;
using Server.DBEntities;
using Server.Managers;
using Server.Servicios;
using Shared.Paquetes;
using Shared.Tipos;
using Shared.Utils;

namespace Server.Handlers
{
    public class LoginHandler : IPacketHandler
    {
        public ServicioAutenticacion _servicio = new();
        public async Task Handle(NetPeer peer, IPaquete paquete)
        {
            if (paquete is not PaquetePeticionInicioSesion peticion)
            {
                return;
            }
            PaqueteRespuestaInicioSesion respuesta = new();
            Jugador? jugador = await _servicio.IniciarSesion(
                peticion.Usuario,
                peticion.Clave,
                respuesta
            );

            if (jugador == null)
            {
                PacketSender.EnviarTCP(peer, respuesta);
                return;
            }

            Sesion sesion = SesionManager.CrearSesion(
                jugador.Id,
                jugador.NombreUsuario,
                peer
            );

            respuesta.Exitoso = true;
            respuesta.Token = sesion.Token;

            PacketSender.EnviarTCP(peer, respuesta);
        }
    }
}