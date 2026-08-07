using LiteNetLib;
using Shared.Paquetes;
namespace Server.Handlers;

public interface IPacketHandler
{
    Task Handle(NetPeer peer, IPaquete paquete);
}