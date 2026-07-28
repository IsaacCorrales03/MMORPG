using LiteNetLib;

namespace Server.Red.Sesiones
{
    public class Sesion
    {
        public string Token { get; set; } = "";
        public int UsuarioId { get; init; }
        public string NombreUsuario { get; init; } = "";
        public NetPeer? Peer { get; set; }
        public DateTime CreadaEn { get; init; }
    }
}