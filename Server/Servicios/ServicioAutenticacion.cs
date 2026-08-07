using Microsoft.EntityFrameworkCore;
using Server.DBEntities;
using Server.Managers;
using Shared.Paquetes;
using Microsoft.AspNetCore.Identity;
namespace Server.Servicios
{
    public class ServicioAutenticacion
    {
        private readonly DataBase db = new();
        private readonly PasswordHasher<Jugador> _passwordHasher = new();

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
            jugador.PasswordHash = _passwordHasher.HashPassword(jugador, clave); ;
            jugador.FechaCreacion = DateTime.UtcNow;
            db.Jugadores.Add(jugador);
            await db.SaveChangesAsync();
            return jugador;
        }

        public async Task<Jugador?> IniciarSesion(string usuario, string clave, PaqueteRespuestaInicioSesion paquete)
        {
            Jugador? jugador = await db.Jugadores
                .FirstOrDefaultAsync(j => j.NombreUsuario == usuario);

            if (jugador == null)
            {
                paquete.Exitoso = false;
                paquete.MensajeError = "Usuario o contraseña incorrectos";
                return null;
            }

            PasswordVerificationResult resultado =
                _passwordHasher.VerifyHashedPassword(
                    jugador,
                    jugador.PasswordHash,
                    clave
                );

            if (resultado == PasswordVerificationResult.Failed)
            {
                paquete.Exitoso = false;
                paquete.MensajeError = "Usuario o contraseña incorrectos";
                return null;
            }

            // Si el algoritmo de hash necesita actualizarse, lo hace automáticamente.
            if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
            {
                jugador.PasswordHash = _passwordHasher.HashPassword(jugador, clave);
                await db.SaveChangesAsync();
            }

            paquete.Exitoso = true;
            return jugador;
        }
    }
}