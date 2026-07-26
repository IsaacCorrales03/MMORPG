using Microsoft.EntityFrameworkCore;
using Server.Core;
using Shared.Paquetes;
using Shared.Utils;
namespace Server.Servicios
{
    public class ServicioAutenticacion
    {
        private readonly DataBase db = new();

        public async Task registrar_jugador(string email, string usuario, string clave, PaqueteRespuestaRegistro paquete)
        {
            Console.WriteLine(Environment.CurrentDirectory);
            Console.WriteLine(db.Database.GetDbConnection().DataSource);
            bool existe_usuario = await db.Jugadores.AnyAsync(jugador => jugador.NombreUsuario == usuario);
            if (existe_usuario)
            {
                paquete.Exitoso = false;
                paquete.MensajeError = "Usuario ya existe";
                
            }
            bool existe_email = await db.Jugadores.AnyAsync(jugador => jugador.Email == email);
            if (existe_email)
            {
                paquete.Exitoso = false;
                paquete.MensajeError = "Email ya registrado";
            }
            Jugador jugador = new();
            jugador.Email = email;
            jugador.NombreUsuario = usuario;
            jugador.PasswordHash = clave;
            jugador.FechaCreacion = DateTime.UtcNow;
            db.Jugadores.Add(jugador);
            
            await db.SaveChangesAsync();
        }
    }
}