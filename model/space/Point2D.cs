using System;


namespace AquariumSimulator.Model.Space
{
    public readonly struct Point2D
    {
        public int X { get; }
        public int Y { get; }


        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public bool Equals(Point2D other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Point2D other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        
        public static bool operator==(Point2D lhs, Point2D rhs)
        {
            return lhs.Equals(rhs);
        }
        
        public static bool operator!=(Point2D lhs, Point2D rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}