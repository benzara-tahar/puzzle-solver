namespace AI_Puzzle_Solver.AIEngine
{
    public static class AIMaths
    {


        public static long Factoriel(int a)
        {
            long b = 1;
            for (int i = 1; i <= a; i++)
            {
                b = b * i;
            }
            return b;
        }

    }
}
