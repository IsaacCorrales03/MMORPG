using LiteNetLib;
using LiteNetLib.Utils;
using MessagePack;
using Shared.Paquetes;

namespace Shared.Utils
{
    public class PacketSender
    {
        public static void EnviarTCP(NetPeer peer, IPaquete paquete)
        {
            // No procesar si el peer ya no está conectado
            if (peer == null || peer.ConnectionState != ConnectionState.Connected)
            {
                return;
            }
            // Serializar el paquete
            byte[] contenido = MessagePackSerializer.Serialize(paquete.GetType(), paquete);
            NetDataWriter writer = new();
            // Añadir el tipo y el contenido del paquete
            writer.Put((byte) paquete.Tipo);
            writer.PutBytesWithLength(contenido);

            // Enviarlo de manera segura
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

    }
}