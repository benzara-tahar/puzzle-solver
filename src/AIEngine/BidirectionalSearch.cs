using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AI_Puzzle_Solver.AIEngine
{
    public class BidirectionalSearch<T> : AISearch<T> where T : class, IVertex
    {

        public BidirectionalSearch(AISearchParameters<T> sp)
        {
            SearchParameter = sp;
        }

        public override void Start()
        {

            BidrectionalSearch();
        }

        void BidrectionalSearch()
        {
            var openedBackward = new Queue<T>();
            var openedForward = new Queue<T>();
            var closed = new Queue<T>();

            var eventArgs = new AISearchEventArgs("Start searching with Bidirectional Search");
            OnSearchStarted(eventArgs);

            openedForward.Enqueue(SearchParameter.StartNode);
            openedBackward.Enqueue(SearchParameter.GoalNode);


            while (openedForward.Count != 0 && !StopFlag)
            {
                var forwardNode = openedForward.Dequeue();

                foreach (T p in forwardNode.Neighbors)
                {

                    p.Parent = forwardNode;
                    if (p.Equals(SearchParameter.GoalNode))
                    {
                        eventArgs.Solution = Solution = ConstructSolution(p);
                        OnSearchCompleted(eventArgs);
                        return;
                    }

                    if (!closed.Contains<T>(p))
                        openedForward.Enqueue(p);

                }

                var backwardNode = openedBackward.Dequeue();

                foreach (T p in backwardNode.Neighbors)
                {
                    p.Parent = backwardNode;

                    if (openedForward.Contains<T>(p))
                    {
                        var mid = openedForward.Single(_ => _.Equals(p));
                        var list = ConstructSolution(p).Reverse().ToList();
                        list.RemoveAt(0);
                        var parent = mid;
                        foreach (T item in list)
                        {
                            item.Parent = parent;
                            parent = item;
                        }

                        eventArgs.Solution = Solution = ConstructSolution(parent);
                        OnSearchCompleted(eventArgs);
                        return;
                    }

                    if (!closed.Contains(p))
                        openedBackward.Enqueue(p);

                }

                closed.Enqueue(forwardNode);
                closed.Enqueue(backwardNode);
                OnSearchProgressChanged(new AISearchEventArgs(closed.Count));
                if (PauseFlag) while (PauseFlag) Thread.Sleep(100);
            }

        }




    }
}
