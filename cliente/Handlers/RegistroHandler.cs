using Godot;
using Client.Juego;
using Shared.Paquetes;
using Client.Red;

namespace Client.Handlers
{
	public static class RegisterHandler
	{
		
		public static void Manejar(PaqueteRespuestaRegistro paquete)
		{
			if (paquete.Exitoso)
			{
				TokenManager.GuardarToken(paquete.Token);
				GameState.Token = paquete.Token;
				GameState.Instance.EmitSignal(GameState.SignalName.RegistroExitoso);
			}
			else
			{
				GameState.Instance.EmitSignal(GameState.SignalName.RegistroFallido);

			}
		}


	}
}
