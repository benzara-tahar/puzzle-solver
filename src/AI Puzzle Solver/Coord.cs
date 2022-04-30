using System;

namespace AI_Puzzle_Solver
{

    class Coord : ICloneable
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Coord p1, Coord p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y);
        }
        public static bool operator !=(Coord p1, Coord p2)
        {
            return !(p1 == p2);
        }


        public object Clone()
        {
            return new Coord(X, Y);
        }

        public override string ToString()
        {
            return "X:" + X + ";   Y:" + Y;
        }
        public override bool Equals(object obj)
        {
            var o = obj as Coord;
            return o == null ? false : o == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
