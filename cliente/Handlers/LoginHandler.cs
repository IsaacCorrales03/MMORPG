using Client.Juego;
using Client.Red;
using Shared.Paquetes;

namespace Client.Handlers
{
    public static class LoginHandler
    {
        public static void Manejar(PaqueteRespuestaInicioSesion paquete)
        {
            if (paquete.Exitoso)
            {
                TokenManager.GuardarToken(paquete.Token);
                GameState.Token = paquete.Token;
                GameState.Instance.EmitSignal(GameState.SignalName.InicioSesionExitoso);
            }
            else
            {
                GameState.Instance.EmitSignal(GameState.SignalName.InicioSesionFallido);
            }
        }
    }
}