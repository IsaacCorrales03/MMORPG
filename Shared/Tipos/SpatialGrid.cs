using Shared.Tipos;

namespace Server.Mundo
{
    public class SpatialGrid
    {
        private readonly Dictionary<ChunkPosition, Chunk> _chunks = new();

        public Chunk GetOrCreateChunk(ChunkPosition position)
        {
            if (!_chunks.TryGetValue(position, out Chunk? chunk))
            {
                chunk = new Chunk(position);
                _chunks[position] = chunk;
            }

            return chunk;
        }
        public bool TryGetChunk(ChunkPosition position, out Chunk chunk)
        {
            return _chunks.TryGetValue(position, out chunk!);
        }
        public void AddPlayer(int playerId, ChunkPosition position)
        {
            Chunk chunk = GetOrCreateChunk(position);
            chunk.Players.Add(playerId);
        }

        public void RemovePlayer(int playerId, ChunkPosition position)
        {
            if (!_chunks.TryGetValue(position, out Chunk? chunk))
                return;

            chunk.Players.Remove(playerId);

            if (chunk.Players.Count == 0)
                _chunks.Remove(position);
        }

        public void MovePlayer(int playerId, ChunkPosition oldPosition, ChunkPosition newPosition)
        {
            if (oldPosition == newPosition)
                return;

            RemovePlayer(playerId, oldPosition);
            AddPlayer(playerId, newPosition);
        }
    }
}