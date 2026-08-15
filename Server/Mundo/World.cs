using Shared.Paquetes;
using Shared.Tipos;

namespace Server.Mundo
{
    public class World
    {
        const float TickDelta = 1f / 20f;
        const float ClientPhysicsDelta = 1f / 60f;

        public readonly Dictionary<int, PlayerState> players = new();
        private readonly HashSet<TipoPaquete> _tiposDeEventos = new() {
            TipoPaquete.Movimiento
        };


        public readonly Dictionary<int, List<IPaquete>> EventPool = new();

        public void AddEvent(IPaquete paquete, int JugadorID)
        {
            if (!EventPool.TryGetValue(JugadorID, out var eventos))
                return;

            if (paquete is PaqueteMovimiento movimiento)
            {
                if (movimiento.Consecutive > 1 && EventPool[JugadorID].Count > 0)
                    EventPool[JugadorID][^1] = movimiento;
                else
                {
                    EventPool[JugadorID].Add(movimiento);
                    return;
                }
            }
            else
            {
                EventPool[JugadorID].Add(paquete);
            }
        }

        public bool ManejaEvento(TipoPaquete tipo)
        {
            return _tiposDeEventos.Contains(tipo);
        }

        public void AddPlayer(int playerId, Vector2 position)
        {
            players[playerId] = new PlayerState(playerId, position);
            EventPool[playerId] = new List<IPaquete>();
        }

        public void RemovePlayer(int playerId)
        {
            players.Remove(playerId);
            EventPool.Remove(playerId);
        }

        public PlayerState? GetPlayer(int playerId)
        {
            players.TryGetValue(playerId, out var player);
            return player;
        }
        private void ProcesarMovimiento(PlayerState player, PaqueteMovimiento movimiento)
        {
            if (movimiento.Moved == false)
            {
                return;
            }
            else
            {
                float DistanciaMaxima = player.MoveSpeed * ClientPhysicsDelta * movimiento.Consecutive;
                Vector2 anterior = player.Position;
                Vector2 reportada = movimiento.ReportedPosition;
                float desplazamiento = anterior.DistanceSquaredTo(reportada);
                if (desplazamiento <= DistanciaMaxima * DistanciaMaxima)
                {
                    player.Position = reportada;
                }
                else
                {
                    Vector2 direccion = (reportada - anterior).Normalized();
                    player.Position = anterior + direccion * DistanciaMaxima;   
                }

            }

        }

        public void Tick()
        {
            foreach (var entry in EventPool)
            {
                int jugadorId = entry.Key;
                List<IPaquete> eventos = entry.Value;

                if (!players.TryGetValue(jugadorId, out var player))
                    continue;

                foreach (var evento in eventos)
                {
                    if (evento is PaqueteMovimiento movimiento)
                    {
                        ProcesarMovimiento(player, movimiento);
                    }
                }

                eventos.Clear();
            }
        }
    }
}