namespace Shared.Tipos
{
    public struct Vector2
    {
        public float X { get; set; }
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
    }
}