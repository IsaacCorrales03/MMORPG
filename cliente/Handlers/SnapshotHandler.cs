using Client.Juego;
using Shared.Paquetes;

namespace Client.Handlers
{
    public static class SnapshotHandler
    {
        public static void Manejar(PaqueteSnapshots paquete)
        {
            GameState.Instance.RecibirSnapshot(paquete);
        }
    }
}