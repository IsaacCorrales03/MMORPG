using System.Text;
using System.Text.Json;
using LiteNetLib;
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

listener.PeerDisconnectedEvent += (peer, disconnectionInfo) =>
{
    Console.WriteLine($"Se desconectó: {peer}, razón: {disconnectionInfo.Reason}");
};

listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) => {
    Console.WriteLine($"{peer} - {peer.Ping} ms");
    byte[] datos = reader.GetRemainingBytes();
    string json = Encoding.UTF8.GetString(datos);
    try
    {
        Sobre? sobre = JsonSerializer.Deserialize<Sobre>(json);

        if (sobre is null)
        {
            Console.WriteLine($"Paquete inválido recibido de {peer}, no se pudo deserializar.");
            reader.Recycle();
            return;
        }
        switch (sobre.TipoDePaquete)
        {
            case TipoPaquete.PeticionInicioSesion:
            {
                var peticion = JsonSerializer.Deserialize<PaquetePeticionInicioSesion>(sobre.Contenido);
                if (peticion is null)
                {
                    Console.WriteLine($"Contenido inválido en PeticionInicioSesion de {peer}");
                    break;
                }
                Console.Write($"{peticion.Usuario} y {peticion.Clave}");
                
                break;
            }
            default:
            {
                Console.WriteLine($"Tipo de paquete no manejado: {sobre.TipoDePaquete}");
                break;
            }
        }
        
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"Error deserializando paquete de {peer}: {ex.Message}");
    }
    
    reader.Recycle();
};

while (!Console.KeyAvailable)
{
    server.PollEvents();
    Thread.Sleep(20);
}
server.Stop();