using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AI_Puzzle_Solver.AIEngine
{
    public class DepthFirstSearch<T> : AISearch<T> where T : class, IVertex
    {
        public DepthFirstSearch(AISearchParameters<T> sp)
        {
            SearchParameter = sp;
        }



        public override void Start()
        {
            var opened = new Stack<T>();
            var closed = new Stack<T>();

            var goal = SearchParameter.GoalNode;
            var eventArgs = new AISearchEventArgs("DepththFirstSearch Started");

            OnSearchStarted(eventArgs);
            opened.Push(SearchParameter.StartNode);

            while (opened.Count != 0 && !StopFlag)
            {
                var node = opened.Pop();

                foreach (T p in node.Neighbors)
                {
                    p.Parent = node;
                    if (p.Equals(goal))
                    {
                        eventArgs.Solution = Solution = ConstructSolution(p);
                        eventArgs.Message = "Solution Found using DFS";
                        OnSearchCompleted(eventArgs);
                        return;
                    }

                    if (!closed.Contains<T>(p))
                        opened.Push(p);
                }

                closed.Push(node);
                if (PauseFlag) while (PauseFlag) Thread.Sleep(100);
                eventArgs.ExploredNodes = closed.Count;
                OnSearchProgressChanged(eventArgs);
            }

        }


    }
}
