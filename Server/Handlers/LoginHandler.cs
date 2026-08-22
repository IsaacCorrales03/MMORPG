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
                PacketSender.EnviarOrdenado(peer, respuesta);
                return;
            }
            ServerLog.Log($"Jugador DB ID: {jugador.Id}");

            Sesion sesion = SesionManager.CrearSesion(
                jugador.Id,
                jugador.NombreUsuario,
                jugador.ClaseId,
                peer
            );

            ServerLog.Log($"Sesion PlayerID: {sesion.UsuarioId}");


            respuesta.Exitoso = true;
            respuesta.Token = sesion.Token;
            respuesta.Username = jugador.NombreUsuario;
            respuesta.IDJugador = jugador.Id;

            PacketSender.EnviarOrdenado(peer, respuesta);
        }
    }
}