using LiteNetLib;
using Server.Managers;
using Server.Mundo;
using Shared.Paquetes;
using Shared.Utils;

namespace Server.Red
{
    public class NetworkServer
    {
        // Instancias de netlib
        private EventBasedNetListener? _listener;
        private NetManager? _server;
        // Instancias del servidor
        public bool Running { get; private set; }
        public int MaximoJugadores { get; }
        public int Puerto { get; }
        public event Action<NetPeer>? JugadorDesconectado;
        public IEnumerable<NetPeer> Peers => _server ?? Enumerable.Empty<NetPeer>();
        private PacketRouter _router;
        // Dependencias del servidor
        // dependencia de router y packet sender

        /// <summary>Cantidad de peers conectados ahora mismo (0 si el servidor no está corriendo).</summary>
        public int JugadoresConectados => _server?.ConnectedPeersCount ?? 0;

        /// <summary>
        /// Estadísticas acumuladas de red (bytes/paquetes enviados y recibidos) desde que
        /// arrancó el NetManager. Null si el servidor no está corriendo. Usalas para calcular
        /// throughput (KB/s) tomando el delta entre dos lecturas.
        /// </summary>
        public NetStatistics? Estadisticas => _server?.Statistics;

        public NetworkServer(int maximoJugadores, int puerto, World world)
        {
            MaximoJugadores = maximoJugadores;
            Puerto = puerto;
            _router = new(world);
        }
        public void Start()
        {
            if (Running)
            {
                return;
            }
            _listener = new();
            // Manejar conexiones y desconexiones
            _listener.PeerConnectedEvent += peer => OnPeerConnected(peer);
            _listener.PeerDisconnectedEvent += (peer, disconnectionInfo) => OnPeerDisconnected(peer, disconnectionInfo);

            // Al intentar conectar...
            _listener.ConnectionRequestEvent += request => OnConnectionRequest(request);

            // Al recibir datos e información desde la red
            _listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) => OnNetworkReceive(peer, reader, channel, deliveryMethod);

            // Nueva instancia del servidor netlib:
            _server = new(_listener)
            {
                EnableStatistics = true // necesario para que Statistics se llene
            };
            if (!_server.Start(Puerto))
            {

                throw new Exception($"No se pudo iniciar el servidor en el puerto {Puerto}.");
            }
            Running = true;
        }
        public void Stop()
        {
            if (!Running)
            {
                return;
            }

            _server?.Stop();

            _listener = null;
            _server = null;

            Running = false;

        }
        public void PollEvents()
        {
            if (_server != null && Running)
            {
                _server.PollEvents();
            }
            else
            {
                throw new Exception("El servidor no está inicializado");
            }

        }

        private void OnPeerConnected(NetPeer peer)
        {
            ServerLog.Log($"+ Cliente conectado: {peer.Address} (id interno: {peer.Id})");
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectionInfo)
        {
            JugadorDesconectado?.Invoke(peer);
            ServerLog.Log($"- Cliente desconectado: {peer.Address} (id interno: {peer.Id}), razón: {disconnectionInfo.Reason}");
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {
            if (_server != null && _server.ConnectedPeersCount < MaximoJugadores)
            {
                // Acepta si tiene la clave
                request.AcceptIfKey(Claves.ClaveServidor);
            }
            else
            {
                // Rechaza si el server está lleno o no tiene la clave
                request.Reject();
            }
        }
        private async void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            try
            {
                TipoPaquete tipo = (TipoPaquete)reader.GetByte();
                byte[] contenido = reader.GetBytesWithLength();
                await _router.Enrutar(tipo, contenido, peer);
            }
            catch (Exception ex)
            {
                ServerLog.Log($"Error procesando paquete de {peer.Address}: {ex.Message}");
            }
            finally
            {
                reader.Recycle();
            }
        }
    }
}