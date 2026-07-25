namespace Tipos
{
    public struct Vector2
    {
        public int Y {get;set;}
        public int X {get;set;}
        
        public Vector2(int y = 0, int x = 0)
        {
            X = x;
            Y = y;
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            int y = a.Y + b.Y;
            int x = a.X + b.X;
            return new Vector2(y, x);
        }
    }
}