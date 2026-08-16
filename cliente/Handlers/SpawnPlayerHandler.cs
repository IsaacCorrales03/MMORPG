using System;
using Client.Juego;
using Client.Red;
using Godot;
using Shared.Paquetes;

namespace Client.Handlers
{
	public class SpawnPlayerHandler
	{
		public static void Manejar(PaqueteRespuestaAparecerJugador paquete)
		{
			if (paquete.Exitoso) {
				GameState.Instance.EmitSignal(GameState.SignalName.AparecerJugador);
			}
			else
			{
				GD.Print($"no emitio: {paquete.MensajeDeError}");
				//GameState.Instance.EmitSignal(GameState.SignalName.SesionReanudadaFallida, paquete.MensajeError ?? "Error desconocido al reanudar sesion");
			}
		}
	}
}
