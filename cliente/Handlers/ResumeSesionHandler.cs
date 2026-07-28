using Client.Juego;
using Shared.Paquetes;

namespace Client.Handlers
{
    public class ResumeSesionHandler
    {
        public static void Manejar(PaqueteRespuestaReanudarSesion paquete)
        {
            if (paquete.Exitoso) {
                GameState.IdUsuario = paquete.IdUsuario;
                GameState.NombreUsuario = paquete.NombreUsuario;
                GameState.Token = paquete.Token;
                GameState.Instance.EmitSignal(GameState.SignalName.SesionReanudadaExitoso);
            }
            else
            {
                GameState.Instance.EmitSignal(GameState.SignalName.SesionReanudadaFallido, paquete.MensajeError ?? "Error desconocido al reanudar sesion");
            }
        }
    }
}