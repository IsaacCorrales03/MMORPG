using LiteNetLib;
using Server.Managers;
using Shared.Clases;
using Shared.Paquetes;
using Shared.Tipos;
using Shared.Utils;

namespace Server.Mundo
{
    public class World
    {
        // ---------- Constantes ----------
        const int AoiChunkRadiusX = 3;
        const int AoiChunkRadiusY = 2;
        private long _currentTick = 0;

        const float ClientPhysicsDelta = 1f / 60f;

        // ---------- Campos ----------
        public readonly SpatialGrid mapa = new();
        public readonly Dictionary<int, PlayerState> players = new();
        public readonly Dictionary<int, List<IPaquete>> EventPool = new();

        private readonly Dictionary<int, PlayerSnapshot> _snapshots = new();
        private readonly HashSet<TipoPaquete> _tiposDeEventos = new() {
            TipoPaquete.Movimiento
        };

        // ---------- Gestión de jugadores ----------
        public void AddPlayer(int playerId, Vector2 position, string nombre, Clase clase)
        {
            players[playerId] = new PlayerState(playerId, position, nombre, clase);
            EventPool[playerId] = new List<IPaquete>();
            mapa.AddPlayer(playerId, players[playerId].chunkPosition);
        }

        public void RemovePlayer(int playerId)
        {
            mapa.RemovePlayer(playerId, players[playerId].chunkPosition);
            players.Remove(playerId);
            EventPool.Remove(playerId);
        }

        public PlayerState? GetPlayer(int playerId)
        {
            players.TryGetValue(playerId, out var player);
            return player;
        }

        // ---------- Gestión de eventos ----------
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
        
        private PlayerSnapshot BuildSnapshot(PlayerState player)
        {
            return new PlayerSnapshot
            {
                PlayerId = player.PlayerId,
                Position = player.Position,
                Nombre = player.Nombre,
                Direction = player.LastDirection,
                Moving = player.IsMoving
            };
        }
        private void BuildSnapshots()
        {
            _snapshots.Clear();

            foreach (PlayerState player in players.Values)
            {
                _snapshots[player.PlayerId] = BuildSnapshot(player);
            }
        }
        private PaqueteSnapshots BuildPacketSnapshots(PlayerState receptor)
        {
            PaqueteSnapshots packet = new()
            {
                Tick = _currentTick,
                LastSequenceProcessed = receptor.LastSequenceProcessed
            };

            ChunkPosition centro = receptor.chunkPosition;

            for (int x = -AoiChunkRadiusX; x <= AoiChunkRadiusX; x++)
            {
                for (int y = -AoiChunkRadiusY; y <= AoiChunkRadiusY; y++)
                {
                    ChunkPosition posicion = new(centro.X + x, centro.Y + y);

                    if (!mapa.TryGetChunk(posicion, out Chunk chunk))
                        continue;

                    foreach (int playerId in chunk.Players)
                    {
                        if (_snapshots.TryGetValue(playerId, out PlayerSnapshot? snapshot))
                        {
                            packet.Players.Add(snapshot);
                        }
                    }
                }
            }
            return packet;
        }
        // ---------- Procesamiento de movimiento ----------
        private void ProcesarMovimiento(PlayerState player, PaqueteMovimiento movimiento)
        {
            player.IsMoving = movimiento.Moved;
            player.LastSequenceProcessed = movimiento.Sequence;
            player.LastDirection = new Vector2(
                movimiento.Input.X,
                movimiento.Input.Y
            );

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
                    PaqueteCorrecionMovimiento paquete = new()
                    {
                        Posicion = player.Position,
                        LastSequenceProcessed = player.LastSequenceProcessed,
                        PlayerId = player.PlayerId
                    };
                    NetPeer? peer = SesionManager.ObtenerPeer(player.PlayerId);
                    if (peer != null)
                    {
                        PacketSender.EnviarOrdenado(peer, paquete);
                    }
                }
                ChunkPosition chunk_anterior = player.chunkPosition;
                ChunkPosition chunk_nuevo = ChunkPosition.FromPosition(player.Position);
                if (chunk_anterior == chunk_nuevo)
                {
                    return;
                }
                else
                {
                    player.chunkPosition = chunk_nuevo;
                    mapa.MovePlayer(player.PlayerId, chunk_anterior, chunk_nuevo);
                }
            }
        }

        // ---------- Ciclo principal ----------
        public void Tick()
        {
            _currentTick++;
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
            BuildSnapshots();
            foreach (PlayerState receptor in players.Values)
            {
                PaqueteSnapshots packet = BuildPacketSnapshots(receptor);

                NetPeer? peer = SesionManager.ObtenerPeer(receptor.PlayerId);

                if (peer != null)
                {
                    PacketSender.EnviarSnapshot(peer, packet);
                }
            }

        }
    }
}