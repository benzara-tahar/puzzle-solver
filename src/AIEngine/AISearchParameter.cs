namespace AI_Puzzle_Solver.AIEngine
{
    public class AISearchParameters<T> where T : IVertex
    {
        public T StartNode { get; set; }
        public T GoalNode { get; set; }
        //public IEqualityComparer<T> Comparer { get; set; }

        public AISearchParameters(T start, T goal)//, IEqualityComparer<T> comparer)
        {
            StartNode = start;
            GoalNode = goal;
            //Comparer = comparer;
        }

        public AISearchParameters()
        {

        }
    }


}
