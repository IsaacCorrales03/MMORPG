using System.Collections.Concurrent;
using LiteNetLib;
using Server.Servicios;
using Shared.Tipos;

namespace Server.Managers
{
    public class SesionManager
    {
        private static readonly ConcurrentDictionary<string, Sesion> _sesionesPorToken = new();
        private static readonly ConcurrentDictionary<int, string> _tokenPorUsuarioId = new();
        private static readonly ConcurrentDictionary<int, NetPeer> _peerPorUsuarioId = new();
        public static Sesion CrearSesion(int UsuarioId, string NombreUsuario,int claseId, NetPeer peer)
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
                ClaseId = claseId,
                CreadaEn = DateTime.UtcNow
            };
            _sesionesPorToken[token] = sesion;
            _tokenPorUsuarioId[UsuarioId] = token;
            _peerPorUsuarioId[UsuarioId] = peer;
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
                _peerPorUsuarioId[sesion.UsuarioId] = nuevoPeer;
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
        public static NetPeer? ObtenerPeer(int UsuarioId)
        {
            _peerPorUsuarioId.TryGetValue(
                UsuarioId,
                out NetPeer? peer
            );

            return peer;
        }
        public static int? ObtenerIdPorPeer(NetPeer peer)
        {
            Sesion? sesion = ObtenerPorPeer(peer);
            return sesion?.UsuarioId;
        }
    }
}