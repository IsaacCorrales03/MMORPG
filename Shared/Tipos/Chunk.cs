namespace Shared.Tipos
{
    public class Chunk
    {
        public ChunkPosition Position { get; }

        public HashSet<int> Players { get; } = new();

        public Chunk(ChunkPosition position)
        {
            Position = position;
        }
    }
}