using LiteNetLib;
using Server.Red.Sesiones;
using Shared.Paquetes;

namespace Server.Handlers
{
    public static class ResumeSessionHandler
    {
        public static async Task<PaqueteRespuestaReanudarSesion> Manejar(PaquetePeticionReanudarSesion peticion, NetPeer peer)
        {
            if (string.IsNullOrEmpty(peticion.Token))
            {
                return new PaqueteRespuestaReanudarSesion
                {
                    Exitoso = false,
                    MensajeError = "Token vacío"
                };
            }

            bool ok = SesionManager.Reanudar(peticion.Token, peer, out Sesion? sesion);

            if (!ok || sesion is null)
            {
                return new PaqueteRespuestaReanudarSesion
                {
                    Exitoso = false,
                    MensajeError = "Sesión inválida o expirada"
                };
            }

            return new PaqueteRespuestaReanudarSesion
            {
                Exitoso = true,
                NombreUsuario = sesion.NombreUsuario,
                Token = sesion.Token,
                IdUsuario = sesion.UsuarioId
            };
        }
    }
}