namespace Shared.Tipos
{
    public readonly record struct ChunkPosition(int X, int Y)
    {
        public const int Size = 512;
        public static ChunkPosition FromPosition(Vector2 position)
        {
            return new ChunkPosition(
                (int) MathF.Floor(position.X / Size),
                (int) MathF.Floor(position.Y / Size)
            );
        }
    }
}