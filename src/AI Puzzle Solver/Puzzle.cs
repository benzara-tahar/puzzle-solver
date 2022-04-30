using AI_Puzzle_Solver.AIEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace AI_Puzzle_Solver
{
    public sealed class Puzzle : ICloneable, IVertex, IComparable<Puzzle>
    {
        #region attribute
        private int[] _map;
        private Puzzle _parent;
        #endregion

        #region properties

        public int RectPos { get; set; }

        public int Size { get; set; }
        #endregion

        #region constructor

        public Puzzle(int size)
        {
            Size = size;
            _map = new int[size * size];
            Reset();
        }
        #endregion

        #region indexors

        public int this[int index]
        {
            get { if (index < Size * Size) return _map[index]; return -1; }
            set { if (index < Size * Size) _map[index] = value; }
        }
        #endregion

        #region methods

        public List<Puzzle> Shuffle(int depth)
        {

            var directions = new[] { Directions.Up, Directions.Right, Directions.Left, Directions.Down };
            var list = new List<Puzzle>();
            var rand = new Random();
            Puzzle track = this;
            Directions lastDirection = Directions.Up, direction;
            for (int i = 0; i < depth; i++)
            {

                //while(( direction = directions[rand.Next() % 4]) == lastDirection )  
                while (((int)(direction = directions[rand.Next() % 4]) + lastDirection) == 0)
                {
                }
                lastDirection = direction;

                var puzzle = track.Clone() as Puzzle;
                if (puzzle != null)
                {
                    puzzle.Move(direction);
                    if (!puzzle.Equals(track))
                    {

                        list.Add(puzzle);
                        track = puzzle;
                    }
                    else i--;
                }
            }

            return list;

        }

        public void Reset()
        {
            for (int i = 0; i < Size * Size; i++)
            {
                _map[i] = i;
            }
            RectPos = Size * Size - 1;

        }

        public void Move(Directions d)
        {
            int oldRectPos = RectPos;
            Coord c = CoordinateConvertor.IndexToCoord(RectPos, Size);
            switch (d)
            {
                case Directions.Up:
                    c.Y++;
                    break;
                case Directions.Down:
                    c.Y--;
                    break;
                case Directions.Left:
                    c.X++;
                    break;
                case Directions.Right:
                    c.X--;
                    break;
                default: return;
            }
            if (c.X < 0 || c.Y < 0 || c.X >= Size || c.Y >= Size) return;
            RectPos = CoordinateConvertor.CoordToIndex(c, Size);
            int temp = this[oldRectPos];
            this[oldRectPos] = this[RectPos];
            this[RectPos] = temp;

        }

        public bool IsSolved()
        {
            for (int i = 0; i < Size * Size; i++)
            {
                if (this[i] != i) return false;
            }
            return true;
        }
        /*
        private bool CanMove(Directions d)
        {
            Coord c = CoordinateConvertor.IndexToCoord(RectPos, Size);
            switch (d)
            {
                case Directions.Up:  c.Y++;  break;
                case Directions.Down: c.Y--; break;
                case Directions.Right: c.X--; break;
                case Directions.Left: c.X++; break;
            }
            return c.X>=0 && c.Y >=0 && c.X<Size && c.Y < Size;
        }
*/
        private IEnumerable<Puzzle> GenerateNeighbors()
        {
            var directions = new Directions[] { Directions.Up, Directions.Right, Directions.Left, Directions.Down };
            var list = new List<Puzzle>();
            foreach (Directions d in directions)
            {
                var p = Clone() as Puzzle;

                p.Move(d);
                if (!p.Equals(Parent) && !p.Equals(this))
                    list.Add(p);
            }

            return list;
        }


        #endregion

        #region Override methods and operators

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var p = obj as Puzzle;
            if (p == null || p.RectPos != RectPos) return false;

            for (int i = 0; i < Size * Size; i++)
            {
                if (this[i] != p[i]) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (_map != null ? _map.GetHashCode() : 0);
                result = (result * 397) ^ RectPos;
                return result;
            }
        }

        public static bool operator ==(Puzzle left, Puzzle right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Puzzle left, Puzzle right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region Interface Implementations

        #region ICloneable interface implementation

        public object Clone()
        {
            var puzzle = new Puzzle(Size) { Parent = Parent };
            for (int i = 0; i < Size * Size; i++)
            {
                puzzle[i] = this[i];
            }
            puzzle.RectPos = RectPos;
            return puzzle;

        }
        #endregion

        #region IVertex interface implementation

        public IVertex Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value as Puzzle;
            }
        }

        public IEnumerable Neighbors
        {
            get { return GenerateNeighbors(); }
        }

        public bool Equal(object v)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable implemantation
        public int CompareTo(Puzzle q)
        {
            var p = this;
            int dp = 0, dq = 0;
            for (int i = 0; i < p.Size * p.Size; i++)
            {
                if (p[i] == i) dp++;
                if (q[i] == i) dq++;
                /*
                dp += Math.Abs(i - p[i]);
                dq += Math.Abs(i - q[i]);
                 */
            }
            for (int i = 0; i < p.Size; i++)
            {
                for (int j = 0; j < p.Size - 1; j++)
                {
                    if (Math.Abs(p[j] - p[i + 1]) == 1) dp++;
                    if (Math.Abs(q[j] - q[i + 1]) == 1) dq++;
                }

            }
            return dq - dp;


        }


        #endregion

        #endregion


    }
}
