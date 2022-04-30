using System.Collections.Generic;

namespace AI_Puzzle_Solver.AIEngine
{
    public delegate void AISearchEventHandler(object sender, AISearchEventArgs args);

    public class AISearch<T> where T : class, IVertex
    {

        #region attributes
        private bool _stopFlag;
        private bool _pauseFlag;
        #endregion

        #region properties
        public AISearchParameters<T> SearchParameter { get; set; }

        public bool StopFlag
        {
            get
            {
                return _stopFlag;
            }
            set
            {
                if (_stopFlag != value)
                {
                    _stopFlag = value;
                    OnSearchStopped(null);
                }
            }
        }

        public bool PauseFlag
        {
            get
            {
                return _pauseFlag;
            }
            set
            {
                if (_pauseFlag != value)
                {
                    _pauseFlag = value;
                    if (value) OnSearchPaused(null); else OnSearchResumed(null);
                }
            }
        }

        public IEnumerable<T> Solution { get; set; }
        #endregion

        #region Events
        public event AISearchEventHandler SearchCompleted;
        public event AISearchEventHandler SearchPaused;
        public event AISearchEventHandler SearchResumed;
        public event AISearchEventHandler SearchStoped;
        public event AISearchEventHandler SearchStarted;
        public event AISearchEventHandler SearchProgressChanged;
        #endregion

        #region events Invocators
        protected virtual void OnSearchCompleted(AISearchEventArgs args)
        {
            AISearchEventHandler handler = SearchCompleted;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnSearchStarted(AISearchEventArgs args)
        {
            AISearchEventHandler handler = SearchStarted;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnSearchPaused(AISearchEventArgs args)
        {
            if (this.SearchPaused != null)
                SearchPaused(this, args);
        }

        protected virtual void OnSearchResumed(AISearchEventArgs args)
        {
            if (this.SearchResumed != null)
                SearchResumed(this, args);
        }

        protected virtual void OnSearchStopped(AISearchEventArgs args)
        {
            if (this.SearchStoped != null)
                SearchStoped(this, args);
        }

        protected virtual void OnSearchProgressChanged(AISearchEventArgs args)
        {
            AISearchEventHandler handler = SearchProgressChanged;
            if (handler != null) handler(this, args);
        }
        #endregion


        #region Constructor

        #endregion

        public virtual void Start() { }

        public void Reset()
        {
            _stopFlag = _pauseFlag = false;
        }

        protected virtual IEnumerable<T> ConstructSolution(T p)
        {
            var solution = new List<T>();
            var track = p;
            while (track != null)
            {
                solution.Add(track);
                track = (T)(track.Parent);
            }
            solution.Reverse();
            return solution;
        }

    }


}
