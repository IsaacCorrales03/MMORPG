using LiteNetLib;
using Server.DBEntities;
using Server.Managers;
using Server.Servicios;
using Shared.Paquetes;
using Shared.Tipos;
using Shared.Utils;

namespace Server.Handlers
{
    public class RegisterHandler : IPacketHandler
    {
        private readonly ServicioAutenticacion _servicio = new();

        public async Task Handle(NetPeer peer, IPaquete paquete)
        {
            if (paquete is not PaquetePeticionRegistro peticion)
            {
                return;
            }

            PaqueteRespuestaRegistro respuesta = new();

            Jugador? jugador = await _servicio.registrar_jugador(
                peticion.Email,
                peticion.Usuario,
                peticion.Clave,
                respuesta
            );

            if (jugador == null)
            {
                PacketSender.EnviarOrdenado(peer, respuesta);
                return;
            }

            Sesion sesion = SesionManager.CrearSesion(
                jugador.Id,
                jugador.NombreUsuario,
                peer
            );

            respuesta.Exitoso = true;
            respuesta.Token = sesion.Token;
            PacketSender.EnviarOrdenado(peer, respuesta);
        }
    }
}