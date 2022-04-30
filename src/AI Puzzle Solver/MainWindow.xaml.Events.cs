using AI_Puzzle_Solver.AIEngine;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AI_Puzzle_Solver
{
    public partial class MainWindow
    {
        #region events
        private void BtnLoadImageClick(object sender, RoutedEventArgs e)
        {

            var ofd = new OpenFileDialog
            {
                Filter = "Image Format(*.jpg,*.png)|*.jpg;*.png|(*.png)|*.png|(*.jpg)|*.jpg|All(*.*)|*.*",
                FilterIndex = 0
            };


            if (!ofd.ShowDialog().GetValueOrDefault()) return;

            imageSource = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
            thumbImage.Source = imageSource;
            resultImage.Source = imageSource;

            var storyboard = Resources["showDlg"] as Storyboard;

            if (storyboard != null) storyboard.Begin(this);

            MouseLeftButtonDown -= Drag;
            KeyUp -= WindowKeyUp;

        }

        private void Start(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null && btn.Name == "btnStart")
            {
                var comboBoxItem = cbFrameCount.SelectedItem as ComboBoxItem;
                var fc = (int)Math.Sqrt(Convert.ToInt32(comboBoxItem.Content.ToString().Substring(0, 2)));
                puzzle = new Puzzle(fc);
                spaceLength = AIMaths.Factoriel(fc * fc);

                rectSize = canvas.ActualWidth / puzzle.Size;
                LoadInitialMap();
                shuffleDepth = Convert.ToInt32(((ComboBoxItem)cbShuffleDepth.SelectedItem).Content.ToString().Substring(0, 2));
                puzzle.Shuffle(shuffleDepth);
                elapsedTime = new TimeSpan(0);
                var shuffleTimer = new DispatcherTimer();
                shuffleTimer.Tick += delegate
                                         {
                                             AnimateMoves(puzzle.Shuffle(shuffleDepth));
                                             shuffleTimer.Stop();
                                             //btnPauseResume.IsEnabled = true;
                                             ((Storyboard)Resources["ShowOriginalImage"]).Begin(this);
                                         };
                shuffleTimer.Interval = TimeSpan.FromMilliseconds(4200);
                shuffleTimer.Start();

                var sb = Resources["3dShowOriginalImage"] as Storyboard;
                sb.Completed += (_, __) => sp.IsEnabled = true;
                sb.Begin(this);

            }

            ((Storyboard)Resources["hideDlg"]).Begin(this);
            MouseLeftButtonDown += Drag;
            KeyUp += WindowKeyUp;


        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            aiSearch.StopFlag = true;
        }

        private void PauseResume(object sender, RoutedEventArgs e)
        {
            aiSearch.PauseFlag = !aiSearch.PauseFlag;
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            puzzle.Reset();
            UpdatePuzzle(puzzle);

            elapsedTime = new TimeSpan(0);
            timer.Stop();
        }

        private void SolvePuzzle(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var sm = btn.Content.ToString().Replace("Solve with", "").Trim();

            ((Storyboard)Resources["ShowSearchDetails"]).Begin(this);
            TBSearchStatus.Text = "Searching...";
            TBSolveMethod.Text = sm;
            TBSpaceLength.Text = spaceLength.ToString();
            var searchParameters = new AISearchParameters<Puzzle>(puzzle, new Puzzle(puzzle.Size));
            switch (sm)
            {
                case "BreadthFirstSearch": aiSearch = new BreadthFirstSearch<Puzzle>(searchParameters); break;
                case "DepthFirstSearch": aiSearch = new DepthFirstSearch<Puzzle>(searchParameters); break;
                case "BestFirstSearch": aiSearch = new BestFirstSearch<Puzzle>(searchParameters); break;
                case "BidirectionalSearch": aiSearch = new BidirectionalSearch<Puzzle>(searchParameters); break;
                default:
                    MessageBox.Show(sm); return;
            }


            //wiring search events
            aiSearch.SearchCompleted += new AISearchEventHandler(SearchCompleted);
            aiSearch.SearchProgressChanged += new AISearchEventHandler(SearchProgressChanged);
            aiSearch.SearchStarted += new AISearchEventHandler(SearchStarted);
            aiSearch.SearchPaused += new AISearchEventHandler(SearchPaused);
            aiSearch.SearchResumed += new AISearchEventHandler(SearchResumed);
            aiSearch.SearchStoped += new AISearchEventHandler(SearchStopped);

            //starting searching on a thread

            Task.Factory.StartNew(aiSearch.Start, TaskCreationOptions.LongRunning);


        }

        #region search events
        private void SearchStopped(object sender, AISearchEventArgs args)
        {
            timer.Stop();
            Dispatcher.BeginInvoke((Action)(() => canvasStopPause.Visibility = Visibility.Hidden));
            Dispatcher.BeginInvoke((Action)(() => sp.IsEnabled = true));
            Dispatcher.BeginInvoke((Action)(() => TBSearchStatus.Text = "Search interrupted."));
        }

        private void SearchResumed(object sender, AISearchEventArgs args)
        {
            btnSwichToOriginl.IsEnabled = true;
            //btnPauseResume.Content = "Pause";
            var da = new DoubleAnimation(0, TimeSpan.FromMilliseconds(100));
            canvas.Effect.BeginAnimation(BlurEffect.RadiusProperty, da);
            timer.Start();
            TBSearchStatus.Text = "Searching...";
        }

        private void SearchPaused(object sender, AISearchEventArgs args)
        {
            btnSwichToOriginl.IsEnabled = false;
            //btnPauseResume.Content = "Resume";
            var da = new DoubleAnimation(10, TimeSpan.FromMilliseconds(100));
            canvas.Effect.BeginAnimation(BlurEffect.RadiusProperty, da);
            timer.Stop();
            TBSearchStatus.Text = "Paused.";
        }

        private void SearchStarted(object sender, AISearchEventArgs args)
        {
            timer.Start();
            Dispatcher.BeginInvoke((Action)(() => sp.IsEnabled = false));
            Dispatcher.BeginInvoke((Action)(() => canvasStopPause.Visibility = Visibility.Visible));
            Dispatcher.BeginInvoke((Action)(() => canvasAnimateSolution.Visibility = Visibility.Hidden));
            Dispatcher.BeginInvoke((Action)(() => TBDescription.Text = ""));
        }

        private void SearchCompleted(object sender, AISearchEventArgs args)
        {
            timer.Stop();
            Dispatcher.BeginInvoke((Action)(() => sp.IsEnabled = true));
            Dispatcher.BeginInvoke((Action)(() => AnimateMoves(aiSearch.Solution)));
            Dispatcher.BeginInvoke((Action)(() => TBSearchStatus.Text = "Solution found."));
            Dispatcher.BeginInvoke((Action)(() => canvasStopPause.Visibility = Visibility.Hidden));
            Dispatcher.BeginInvoke((Action)(() => canvasAnimateSolution.Visibility = Visibility.Visible));
            Dispatcher.BeginInvoke(
               (Action)
               (() => TBDescription.Text = "Solution length: " + ((List<Puzzle>)args.Solution).Count));
        }

        private void SearchProgressChanged(object sender, AISearchEventArgs args)
        {
            Dispatcher.BeginInvoke((Action)(() => TBExplredNodes.Text = args.ExploredNodes.ToString()));
            Dispatcher.BeginInvoke((Action)(() => TBProgress.Text = (String.Format("{0:00.0000}%", (double)args.ExploredNodes / spaceLength))));

        }

        #endregion


        private void Drag(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ExitMinumizeButtonClick(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.ToString().ToUpper() == "X") this.Close();
            else
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void ShowOriginalImage(object sender, RoutedEventArgs e)
        {
            btnSwichToOriginl.IsEnabled = false;
            var sb = Resources["3dShowOriginalImage"] as Storyboard;
            sb.Completed += (o, O) => btnSwichToOriginl.IsEnabled = true;
            sb.Begin(this);
        }

        private void Shuffle(object sender, RoutedEventArgs e)
        {
            AnimateMoves(puzzle.Shuffle(shuffleDepth));
        }

        private void WindowKeyUp(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Left: puzzle.Move(Directions.Left); break;
                case Key.Up: puzzle.Move(Directions.Up); break;
                case Key.Down: puzzle.Move(Directions.Down); break;
                case Key.Right: puzzle.Move(Directions.Right); break;
            }
            UpdatePuzzle(puzzle);
            //textBoxMovesCount.Text = puzzle.GetMovesCount().ToString();

            var sb = Resources["move_wav"] as Storyboard;
            if (soundOn.IsChecked.GetValueOrDefault()) sb.Begin(this);
            if (puzzle.IsSolved()) MessageBox.Show("Puzzle Solved ");

        }

        #endregion
    }
}
