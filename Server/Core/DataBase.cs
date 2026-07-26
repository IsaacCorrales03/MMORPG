using Microsoft.EntityFrameworkCore;

namespace Server.Core
{
    public class DataBase : DbContext
    {
        public DbSet<Jugador> Jugadores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=juego.db");
        }
    }
}