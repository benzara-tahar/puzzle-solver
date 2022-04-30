
using System.Collections;

namespace AI_Puzzle_Solver.AIEngine
{
    public interface IVertex
    {
        IVertex Parent { get; set; }
        IEnumerable Neighbors { get; }

    }
}
