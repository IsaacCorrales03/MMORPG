using System.Text;
using System.Text.Json;
using LiteNetLib;
using Server.Red;
using Shared.Paquetes;
using Shared.Utils;

const int MAXIMO_JUGADORES = 20;

var listener = new EventBasedNetListener();
var server = new NetManager(listener);

server.Start(8455);
// esto es suscribirme a cuando una conexión es pedida (connection request)
listener.ConnectionRequestEvent += request =>
{
    // desicidimos si aceptar o no en base a:
    if (server.ConnectedPeersCount >= MAXIMO_JUGADORES)
    {
        // maximo de jugadores (20)
        request.Reject();
    }
    else
    {
        request.AcceptIfKey(Claves.ClaveServidor);
    }
};

listener.PeerConnectedEvent += peer =>
{
    Console.WriteLine($"Nueva conexión {peer}");
};

listener.PeerDisconnectedEvent += async (peer, disconnectionInfo) =>
{
    Console.WriteLine($"Se desconectó: {peer}, razón: {disconnectionInfo.Reason}");
};

listener.NetworkReceiveEvent += async (peer, reader, channel, deliveryMethod) =>
{
    try
    {
        TipoPaquete tipo = (TipoPaquete)reader.GetByte();
        byte[] contenido = reader.GetBytesWithLength();

        await Router.Enrutar(tipo, contenido, peer);
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"Error deserializando paquete de {peer}: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error inesperado procesando paquete de {peer}: {ex}");
    }
    finally
    {
        reader.Recycle();
    }
};

while (!Console.KeyAvailable)
{
    server.PollEvents();
    Thread.Sleep(20);
}
server.Stop();