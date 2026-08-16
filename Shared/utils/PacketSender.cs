using LiteNetLib;
using LiteNetLib.Utils;
using MessagePack;
using Shared.Paquetes;

namespace Shared.Utils
{
    public class PacketSender
    {
        public static void EnviarOrdenado(NetPeer peer, IPaquete paquete)
        {
            // No procesar si el peer ya no está conectado
            if (peer == null || peer.ConnectionState != ConnectionState.Connected)
            {
                return;
            }
            Console.WriteLine($">Enviando: {paquete.Tipo}");
            // Serializar el paquete
            byte[] contenido = MessagePackSerializer.Serialize(paquete.GetType(), paquete);
            NetDataWriter writer = new();
            // Añadir el tipo y el contenido del paquete
            writer.Put((byte)paquete.Tipo);
            writer.PutBytesWithLength(contenido);

            // Enviarlo de manera segura
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
        public static void EnviarMovimiento(NetPeer peer, PaqueteMovimiento paquete)
        {
            if (peer == null || peer.ConnectionState != ConnectionState.Connected)
            {
                return;
            }
            Console.WriteLine($">Enviando: {paquete.Tipo}");
            byte[] contenido = MessagePackSerializer.Serialize(paquete);
            NetDataWriter writer = new();

            writer.Put((byte)paquete.Tipo);
            writer.PutBytesWithLength(contenido);

            peer.Send(writer, DeliveryMethod.Unreliable);
        }
        public static void EnviarSnapshot(NetPeer peer, PaqueteSnapshots paquete)
        {
            if (peer == null ||
                peer.ConnectionState != ConnectionState.Connected)
            {
                return;
            }

            byte[] contenido = MessagePackSerializer.Serialize(paquete);

            NetDataWriter writer = new();

            writer.Put((byte)paquete.Tipo);
            writer.PutBytesWithLength(contenido);

            peer.Send(writer, DeliveryMethod.Sequenced);
        }
    }
}