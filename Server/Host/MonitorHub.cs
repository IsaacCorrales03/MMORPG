using Microsoft.AspNetCore.SignalR;

namespace Server.Host
{
    public class MonitorHub : Hub
    {
        // El servidor empuja datos, el cliente no necesita invocar métodos por ahora
    }
}