using System.Collections.Concurrent;
using LiteNetLib;

namespace Server.Red.Sesiones
{
    public class SesionManager
    {
        private static readonly ConcurrentDictionary<string, Sesion> _sesionesPorToken = new();
        private static readonly ConcurrentDictionary<int, string> _tokenPorUsuarioId = new();
        public static Sesion CrearSesion(int UsuarioId, string NombreUsuario, NetPeer peer)
        {
            // Si el usuario ya tenía una sesión activa (login desde otro lado), la invalidamos primero
            if (_tokenPorUsuarioId.TryRemove(UsuarioId, out string? tokenViejo))
            {
                // eliminamos la sesion a ese token, ya que eliminamos el token
                _sesionesPorToken.TryRemove(tokenViejo, out _);
            }
            string token = TokenGenerator.Generar();
            Sesion sesion = new()
            {
                Token = token,
                UsuarioId = UsuarioId,
                NombreUsuario = NombreUsuario,
                Peer = peer,
                CreadaEn = DateTime.UtcNow
            };
            _sesionesPorToken[token] = sesion;
            _tokenPorUsuarioId[UsuarioId] = token;
            peer.Tag = sesion;
            return sesion;
        }

        public static bool Reanudar(string tokenViejo, NetPeer nuevoPeer, out Sesion? sesion)
        {
            if (_sesionesPorToken.TryRemove(tokenViejo, out sesion))
            {
                string tokenNuevo = TokenGenerator.Generar();
                sesion.Token = tokenNuevo;
                _sesionesPorToken[tokenNuevo] = sesion;
                _tokenPorUsuarioId[sesion.UsuarioId] = tokenNuevo;

                sesion.Peer = nuevoPeer;
                nuevoPeer.Tag = sesion;
                return true;
            }
            sesion = null;
            return false;
        } 
        public static void CerrarSesion(string token)
        {
            if (_sesionesPorToken.TryRemove(token, out Sesion? sesion))
            {
                _tokenPorUsuarioId.TryRemove(sesion.UsuarioId, out _);
            }
        }
        public static Sesion? ObtenerPorPeer(NetPeer peer) => peer.Tag as Sesion;
    }
}