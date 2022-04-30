using AI_Puzzle_Solver.AIEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AI_Puzzle_Solver
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        #region attributes
        // our puzzle
        Puzzle puzzle;
        //width (and heigh because there are equal 'square') of a piece of the image
        double rectSize;

        int shuffleDepth;

        Random rand = new Random();

        ImageSource imageSource;

        private long spaceLength;

        DispatcherTimer timer = new DispatcherTimer();

        //used to calculated the time took to find a solution
        TimeSpan elapsedTime = new TimeSpan(0);

        private AISearch<Puzzle> aiSearch;
        //used to show the solution move by move
        private int currentMove = 0;
        private bool playing;
        #endregion

        #region properties

        public bool SearchOn { get; set; }
        #endregion

        #region construcor
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += (o, O) => UpdateTime();
            imageSource = resultImage.Source;
            puzzle = new Puzzle(3);
            LoadInitialMap();
        }
        #endregion

        #region methods
        private void UpdateTime()
        {
            elapsedTime = elapsedTime.Add(timer.Interval);
            TextBoxtimer.Text = elapsedTime.ToString("c");
        }

        private void UpdatePuzzle(Puzzle p)
        {
            for (int i = 0; i < p.Size * p.Size; i++)
            {
                var rect = canvas.Children.Cast<Rectangle>().Single<Rectangle>((r) => r.Name == "rect" + p[i]);

                var dax = new DoubleAnimation(i % p.Size * rectSize, TimeSpan.FromMilliseconds(100));
                var day = new DoubleAnimation(i / p.Size * rectSize, TimeSpan.FromMilliseconds(100));
                rect.BeginAnimation(Canvas.LeftProperty, dax);
                rect.BeginAnimation(Canvas.TopProperty, day);
            }
        }

        private void LoadInitialMap()
        {
            canvas.Children.Clear();
            var img = imageSource as BitmapSource;
            int iw = img.PixelWidth / puzzle.Size;
            int ih = img.PixelHeight / puzzle.Size;
            for (int i = 0; i < puzzle.Size * puzzle.Size; i++)
            {

                var rect = new Rectangle { Width = rectSize, Height = rectSize, StrokeThickness = 1, Stroke = Brushes.DarkGray, Name = "rect" + i, RadiusX = 2, RadiusY = 2 };
                var cb = new CroppedBitmap(img, new Int32Rect((i % puzzle.Size) * iw, (i / puzzle.Size) * ih, iw, ih));
                rect.Fill = new ImageBrush(cb) { Stretch = Stretch.Fill };
                canvas.Children.Add(rect);
                var coord = CoordinateConvertor.IndexToCoord(i, puzzle.Size);
                Canvas.SetLeft(rect, coord.X * rectSize);
                Canvas.SetTop(rect, coord.Y * rectSize);
                if (i == puzzle.Size * puzzle.Size - 1)
                {
                    rect.Fill.Opacity = .2;
                    rect.StrokeThickness = 2;
                    rect.RadiusX = rect.RadiusY = 3;
                    rect.Stroke = Brushes.White;
                }
            }
            UpdatePuzzle(puzzle);
        }



        #endregion

        private void About(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("coded with love by @BenzaraTahar Benlahcene @ 2014");
        }

        private void Close(object sender, RoutedEventArgs e)
        {

            Close();
        }

        private void PlayPause(object sender, RoutedEventArgs e)
        {

            playing = currentMove == 0 || !playing;
            if (playing)
            {
                ((Image)btnPlayPause.Content).Source = new BitmapImage(new Uri(@"\Resources\pause.png", UriKind.Relative));
                AnimateMoves(aiSearch.Solution);
                //                btnNext.IsEnabled = btnPrevious.IsEnabled = false;
            }
            else
            {
                ((Image)btnPlayPause.Content).Source = new BitmapImage(new Uri(@"\Resources\play.png", UriKind.Relative));
                //              btnNext.IsEnabled = btnPrevious.IsEnabled = true;
            }
            btnNext.IsEnabled = btnPrevious.IsEnabled = !playing;

        }

        private void AnimateNextMove(object sender, RoutedEventArgs e)
        {

            currentMove++;
            if (currentMove >= aiSearch.Solution.Count()) currentMove = 0;

            textBoxMovesCount.Text = (currentMove + 1).ToString();
            if (currentMove == aiSearch.Solution.Count() - 1)
                btnNext.IsEnabled = false;

            UpdatePuzzle(aiSearch.Solution.ToList().ElementAt(currentMove));

            btnPrevious.IsEnabled = true;

        }

        private void AnimatePreviousMove(object sender, RoutedEventArgs e)
        {
            currentMove--;
            if (currentMove < 0) currentMove = aiSearch.Solution.Count() - 1;
            textBoxMovesCount.Text = (currentMove + 1).ToString();

            if (currentMove == 0)
                btnPrevious.IsEnabled = false;

            UpdatePuzzle(aiSearch.Solution.ToList().ElementAt(currentMove));
            UpdatePuzzle(aiSearch.Solution.ToList().ElementAt(currentMove));
            btnNext.IsEnabled = true;

        }

        void AnimateMoves(IEnumerable<Puzzle> moves)
        {
            int i = 0;
            Storyboard sb = this.Resources[(object)"move_wav"] as Storyboard;
            new Thread((ParameterizedThreadStart)(_ =>
            {
                foreach (Puzzle move in moves)
                {
                    ++i;
                    this.Dispatcher.BeginInvoke((Delegate)new Action<Puzzle>(this.UpdatePuzzle), (object)move);
                    this.Dispatcher.BeginInvoke((Action)(() => this.textBoxMovesCount.Text = i.ToString()));
                    this.Dispatcher.BeginInvoke((Action)(() => sb.Begin((FrameworkElement)this)));
                    Thread.Sleep(200);
                }
            })).Start();

        }


    }


}
