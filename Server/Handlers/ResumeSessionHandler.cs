using LiteNetLib;
using Shared.Tipos;
using Shared.Paquetes;
using Server.Managers;
using Shared.Utils;

namespace Server.Handlers
{
    public class ResumeSessionHandler : IPacketHandler
    {
        public async Task Handle(NetPeer peer, IPaquete paquete)
        {
            if (paquete is not PaquetePeticionReanudarSesion peticion)
            {
                return;
            }
            PaqueteRespuestaReanudarSesion respuesta = new();
            if (string.IsNullOrEmpty(peticion.Token))
            {
                respuesta.Exitoso = false;
                respuesta.MensajeError = "Token Vacío";
                PacketSender.EnviarOrdenado(peer, respuesta);
                return;
            }

            bool ok = SesionManager.Reanudar(peticion.Token, peer, out Sesion? sesion);

            if (!ok || sesion is null)
            {
                respuesta.Exitoso = false;
                respuesta.MensajeError = "Sesión inválida o expirada";
                ServerLog.Log($"⚠ Intento de reanudar sesión inválido/expirado (peer {peer.Id})");
                PacketSender.EnviarOrdenado(peer, respuesta);
                return;
            }
            respuesta.Exitoso = true;
            respuesta.NombreUsuario = sesion.NombreUsuario;
            respuesta.IdUsuario = sesion.UsuarioId;
            respuesta.Token = sesion.Token;
            ServerLog.Log($"✓ Sesión reanudada: '{sesion.NombreUsuario}'");
            PacketSender.EnviarOrdenado(peer, respuesta);
        }
    }
}