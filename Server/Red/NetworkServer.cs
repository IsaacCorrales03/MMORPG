using System.ComponentModel;
using System.Text.Json;
using LiteNetLib;
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

        // Dependencias del servidor
        // dependencia de router y packet sender
        public NetworkServer(int maximoJugadores, int puerto)
        {
            MaximoJugadores = maximoJugadores;
            Puerto = puerto;
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
            _server = new(_listener);
            if (!_server.Start(Puerto))
            {

                throw new Exception($"No se pudo iniciar el servidor en el puerto {Puerto}.");
            }
            Running = true;
            Console.WriteLine("====================================");
            Console.WriteLine("Astera Server Iniciado correctamente");
            Console.WriteLine($"Puerto: {Puerto}");
            Console.WriteLine($"Maximo de jugadores: {MaximoJugadores}");
            Console.WriteLine("====================================");
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

            Console.WriteLine("====================================");
            Console.WriteLine("Astera Server detenido.");
            Console.WriteLine("====================================");
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
            Console.WriteLine($"Nuevo cliente: {peer}");
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectionInfo)
        {
            Console.WriteLine($"Cliente desconectado: {peer}, razón: {disconnectionInfo.Reason}");
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {
            if ( _server != null && _server.ConnectedPeersCount < MaximoJugadores)
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
                Console.WriteLine($">> Paquete {tipo} recibido desde {peer}");
                await PacketRouter.Enrutar(tipo, contenido, peer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado procesando paquete de {peer}: {ex}");
            }
            finally
            {
                reader.Recycle();
            }
        }
    }
}