using MessagePack;

namespace Shared.Tipos
{
    [MessagePackObject]
    public struct Vector2
    {
        [Key(0)]
        public float X { get; set; }
        [Key(1)]
        public float Y { get; set; }

        public Vector2(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            float x = a.X + b.X;
            float y = a.Y + b.Y;
            return new Vector2(x, y);
        }
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(
                a.X - b.X,
                a.Y - b.Y
            );
        }
        public float DistanceTo(Vector2 other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;

            return MathF.Sqrt(dx * dx + dy * dy);
        }
        public float DistanceSquaredTo(Vector2 other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;

            return dx * dx + dy * dy;
        }
        public float Length()
        {
            return MathF.Sqrt(X * X + Y * Y);
        }

        public Vector2 Normalized()
        {
            float length = Length();

            if (length == 0)
                return new Vector2();

            return new Vector2(
                X / length,
                Y / length
            );
        }

        public static Vector2 operator *(Vector2 vector, float scalar)
        {
            return new Vector2(
                vector.X * scalar,
                vector.Y * scalar
            );
        }
    }
}