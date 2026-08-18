using Client.Juego;
using Client.Red;
using Godot;
using Shared.Paquetes;

namespace Client.Handlers
{
    public static class CorrecionMovimientoHandler
    {
        public static void Manejar(PaqueteCorrecionMovimiento paquete)
        {
            Vector2 posicion = new Vector2(paquete.Posicion.X, paquete.Posicion.Y);
            GameState.Instance.EmitSignal(GameState.SignalName.CorregirMovimiento, paquete.PlayerId, paquete.LastSequenceProcessed, posicion);
        }
    }
}