using System;
using System.Collections;

namespace AI_Puzzle_Solver.AIEngine
{
    public class AISearchEventArgs : EventArgs
    {
        public IEnumerable Solution { get; set; }
        public string Message { get; set; }
        public double CurrentProgress { get; set; }
        public long ExploredNodes { get; set; }

        public AISearchEventArgs(string message)
        {
            Message = message;
        }
        public AISearchEventArgs(long en)
        {
            ExploredNodes = en;
        }
        public AISearchEventArgs(double cp)
        {
            CurrentProgress = cp;
        }

        public AISearchEventArgs()
        {
            ExploredNodes = 0;
            CurrentProgress = 0.0f;
        }
        public AISearchEventArgs(long exploredNodes, string message, IEnumerable solution) : this(exploredNodes)
        {
            Message = message;
            Solution = solution;
        }

    }
}
