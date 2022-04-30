using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AI_Puzzle_Solver.AIEngine
{
    public class BestFirstSearch<T> : AISearch<T> where T : class, IVertex, IComparable<T>
    {

        public BestFirstSearch(AISearchParameters<T> sp)
        {
            SearchParameter = sp;

        }
        public override void Start()
        {
            SortedSet<T> opened = new SortedSet<T>();

            List<T> closed = new List<T>();

            var eventArgs = new AISearchEventArgs("BestFirstSearch Started");
            OnSearchStarted(eventArgs);


            opened.Add(SearchParameter.StartNode);
            while (opened.Count != 0 && !StopFlag)
            {
                var node = opened.ElementAt(0);
                foreach (T p in node.Neighbors)
                {
                    p.Parent = node;
                    if (p.Equals(SearchParameter.GoalNode))
                    {

                        eventArgs.Solution = Solution = ConstructSolution(p);
                        eventArgs.Message = "Solution Found using BestFS";
                        OnSearchCompleted(eventArgs);
                        return;
                    }

                    if (!closed.Contains<T>(p))
                        opened.Add(p);


                }

                closed.Add(node);
                opened.Remove(node);
                if (PauseFlag) while (PauseFlag) Thread.Sleep(100);
                eventArgs.ExploredNodes = closed.Count;
                OnSearchProgressChanged(eventArgs);
            }
            //Dispatcher.BeginInvoke((Action)(() => TBSearchStatus.Text = "Search Stopped."));

        }
    }
}
