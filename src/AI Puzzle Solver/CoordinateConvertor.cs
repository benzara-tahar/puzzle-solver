namespace AI_Puzzle_Solver
{
    static class CoordinateConvertor
    {
        public static int CoordToIndex(int x, int y, int mapSize)
        {
            return x + y * mapSize;
        }
        public static int CoordToIndex(Coord c, int mapSize)
        {
            return c.X + c.Y * mapSize;
        }

        public static Coord IndexToCoord(int index, int mapSize)
        {
            return new Coord(index % mapSize, index / mapSize); ;
        }
    }
}
