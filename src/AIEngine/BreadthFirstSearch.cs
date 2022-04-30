using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AI_Puzzle_Solver.AIEngine
{
    public class BreadthFirstSearch<T> : AISearch<T> where T : class, IVertex
    {
        public BreadthFirstSearch(AISearchParameters<T> sp)
        {
            SearchParameter = sp;
        }



        public override void Start()
        {
            var opened = new Queue<T>();
            var closed = new Queue<T>();

            var goal = SearchParameter.GoalNode;
            var eventArgs = new AISearchEventArgs("BreadthFirstSearch Started");

            OnSearchStarted(eventArgs);
            opened.Enqueue(SearchParameter.StartNode);

            while (opened.Count != 0 && !StopFlag)
            {
                var node = opened.Dequeue();

                foreach (T p in node.Neighbors)
                {
                    p.Parent = node;
                    if (p.Equals(goal))
                    {
                        eventArgs.Solution = Solution = ConstructSolution(p);
                        eventArgs.Message = "Solution Found using BFS";
                        OnSearchCompleted(eventArgs);
                        return;
                    }

                    if (!closed.Contains<T>(p))
                        opened.Enqueue(p);
                }

                closed.Enqueue(node);
                if (PauseFlag) while (PauseFlag) Thread.Sleep(100);
                eventArgs.ExploredNodes = closed.Count;
                OnSearchProgressChanged(eventArgs);
            }

        }


    }
}
