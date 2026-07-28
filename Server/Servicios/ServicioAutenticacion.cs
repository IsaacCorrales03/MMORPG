using Microsoft.EntityFrameworkCore;
using Server.Core;
using Shared.Paquetes;

namespace Server.Servicios
{
    public class ServicioAutenticacion
    {
        private readonly DataBase db = new();

        public async Task<Jugador?> registrar_jugador(string email, string usuario, string clave, PaqueteRespuestaRegistro paquete)
        {
            bool existe_usuario = await db.Jugadores.AnyAsync(jugador => jugador.NombreUsuario == usuario);
            if (existe_usuario)
            {
                paquete.Exitoso = false;
                paquete.MensajeError = "Usuario ya existe";
                return null;
                
            }
            bool existe_email = await db.Jugadores.AnyAsync(jugador => jugador.Email == email);
            if (existe_email)
            {
                paquete.Exitoso = false;
                Console.Write("registrado");
                paquete.MensajeError = "Email ya registrado";
                return null;
            }
            Jugador jugador = new();
            jugador.Email = email;
            jugador.NombreUsuario = usuario;
            jugador.PasswordHash = clave;
            jugador.FechaCreacion = DateTime.UtcNow;
            db.Jugadores.Add(jugador);
            await db.SaveChangesAsync();
            return jugador;
        }
    }
}