using Microsoft.EntityFrameworkCore;
using Server.DBEntities;

namespace Server.Managers
{
    public class DataBase : DbContext
    {
        public DbSet<Jugador> Jugadores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=game.db");
        }
    }
}