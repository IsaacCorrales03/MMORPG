namespace Server.DBEntities
{
    public class Jugador
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public int ClaseId { get; set; }
    }
}