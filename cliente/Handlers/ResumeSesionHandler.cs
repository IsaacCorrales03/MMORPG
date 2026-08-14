using System;
using Client.Juego;
using Client.Red;
using Godot;
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
				TokenManager.GuardarToken(paquete.Token);
				GameState.Token = paquete.Token;
				GameState.Instance.EmitSignal(GameState.SignalName.SesionReanudadaExitosa);
			}
			else
			{
				GD.Print($"no emitio: {paquete.MensajeError}");
				GameState.Instance.EmitSignal(GameState.SignalName.SesionReanudadaFallida, paquete.MensajeError ?? "Error desconocido al reanudar sesion");
			}
		}
	}
}
