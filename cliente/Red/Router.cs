using System;
using System.Collections.Generic;
using Client.Handlers;
using Godot;
using LiteNetLib;
using MessagePack;
using Shared.Paquetes;

namespace Client.Red
{
	public static class Router
	{
		private static readonly Dictionary<TipoPaquete, Type> TiposDePaquete = new()
		{
			{ TipoPaquete.RespuestaRegistro, typeof(PaqueteRespuestaRegistro) },
			{ TipoPaquete.RespuestaInicioSesion, typeof(PaqueteRespuestaInicioSesion) },
			{ TipoPaquete.RespuestaReanudarSesion, typeof(PaqueteRespuestaReanudarSesion) }
		};

		public static void Enrutar(TipoPaquete tipoPaquete, byte[] contenido, NetPeer peer)
		{
			if (!TiposDePaquete.TryGetValue(tipoPaquete, out Type clasePaquete))
			{
				GD.Print($"Tipo de paquete no registrado: {tipoPaquete}");
				return;
			}

			object resultado;

			try
			{
				resultado = MessagePackSerializer.Deserialize(clasePaquete, contenido);
			}
			catch (Exception ex)
			{
				GD.Print($"Error deserializando paquete {tipoPaquete}: {ex.Message}");
				return;
			}

			switch (resultado)
			{
				case PaqueteRespuestaRegistro respuesta:
					RegisterHandler.Manejar(respuesta);
					break;

				case PaqueteRespuestaInicioSesion respuesta:
					LoginHandler.Manejar(respuesta);
					break;

				case PaqueteRespuestaReanudarSesion respuesta:
					ResumeSesionHandler.Manejar(respuesta);
					break;

				default:
					GD.Print($"No existe un handler para {tipoPaquete}");
					break;
			}
		}
	}
}
