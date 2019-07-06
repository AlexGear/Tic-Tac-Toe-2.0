using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;
using Microsoft.Phone.Controls;


namespace Крестики_нолики_2._0 {
    public partial class Page1 : PhoneApplicationPage {
        private int fieldSize;
        private double linesWidth;
        private ManipulationHandler manipulationHandler;
        private int redScore;
        private int blueScore;
        private bool gameOver = false;

        public Page1() {
            InitializeComponent();
        }
        
        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e) {
            if(!gameOver) {
                e.Cancel = true;
                if(MessageBox.Show(Consts.SureToExit, Consts.SureToExitTitle, MessageBoxButton.OKCancel) == 
                    MessageBoxResult.OK) {
                    e.Cancel = false;
                }
            }
        }

        private void SetCurrentPlayer(Player currentPlayer) {
            const string yourMove = Consts.YourMoveString;
            const string redMove = Consts.RedPlayerMoveString;
            const string blueMove = Consts.BluePlayerMoveString;
            currentMoveTextR.Text = currentPlayer == Player.Red ? yourMove : blueMove;
            currentMoveTextB.Text = currentPlayer == Player.Blue ? yourMove : redMove;
        }

        private void ChangeScore(Player player) {
            if(player == Player.Red) {
                redScore++;
                redScoreText1.Text = redScore.ToString();
                redScoreText2.Text = redScore.ToString();
            }
            else {
                blueScore++;
                blueScoreText1.Text = blueScore.ToString();
                blueScoreText2.Text = blueScore.ToString();
            }
        }

        private void GameOver() {
            gameOver = true;
            double width = LayoutRoot.ActualWidth;
            double height = LayoutRoot.ActualHeight / 3;
            double textOffset = height / 3;
            double redGridTranslateStart = LayoutRoot.ActualHeight;
            double redGridTranslateEnd = LayoutRoot.ActualHeight - height - 50;
            double blueGridTranslateStart = -height;
            double blueGridTranslateEnd = 50;
            Grid redGrid = new Grid() {
                RenderTransform = new TranslateTransform(),
                Background = new SolidColorBrush(ConstantColors.GameOverGridBackground),
                Width = width, Height = height,
                VerticalAlignment = VerticalAlignment.Top
            };
            Grid blueGrid = new Grid() {
                RenderTransform = new TranslateTransform(),
                Background = new SolidColorBrush(ConstantColors.GameOverGridBackground),
                Width = width, Height = height,
                VerticalAlignment = VerticalAlignment.Top
            };
            TextBlock redText = new TextBlock() {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, textOffset, 0, 0),
                FontFamily = new FontFamily("Segoe WP"),
                FontSize = 30.0
            };
            TextBlock blueText = new TextBlock() {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, textOffset, 0, 0),
                FontFamily = new FontFamily("Segoe WP"),
                FontSize = 30.0
            };
            blueText.Loaded += (sender, e) => {
                blueText.RenderTransform = new RotateTransform() {
                    CenterX = blueText.ActualWidth / 2, CenterY = blueText.ActualHeight, Angle = 180
                };
            };
            if(redScore > blueScore) {
                redText.Text = string.Format(Consts.YouWonFormat, redScore, blueScore);
                blueText.Text = string.Format(Consts.YouLoseFormat, blueScore, redScore);
            }
            else if(redScore == blueScore) {
                redText.Text = blueText.Text = Consts.DeadHeatString;
            }
            else {
                blueText.Text = string.Format(Consts.YouWonFormat, blueScore, redScore);
                redText.Text = string.Format(Consts.YouLoseFormat, redScore, blueScore);
            }
            redGrid.Children.Add(redText);
            blueGrid.Children.Add(blueText);
            LayoutRoot.Children.Add(redGrid);
            LayoutRoot.Children.Add(blueGrid);
            Storyboard storyboard = new Storyboard();
            DoubleAnimation anim = new DoubleAnimation() {
                From = redGridTranslateStart, To = redGridTranslateEnd,
                Duration = TimeSpan.FromMilliseconds(Consts.GridsEmergenceDuration),
                EasingFunction = new BackEase() { Amplitude = 0.2, EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(anim, redGrid);
            Storyboard.SetTargetProperty(anim, new PropertyPath("(UIElement.RenderTransform).Y"));
            storyboard.Children.Add(anim);
            anim = new DoubleAnimation() {
                From = blueGridTranslateStart, To = blueGridTranslateEnd,
                Duration = TimeSpan.FromMilliseconds(Consts.GridsEmergenceDuration),
                EasingFunction = new BackEase() { Amplitude = 0.2, EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(anim, blueGrid);
            Storyboard.SetTargetProperty(anim, new PropertyPath("(UIElement.RenderTransform).Y"));
            storyboard.Children.Add(anim);
            storyboard.Begin();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) {
            fieldSize = Convert.ToInt32(NavigationContext.QueryString["FieldSize"]);
            string firstMoveString = NavigationContext.QueryString["FirstMove"];
            FirstMovePlayer first = (FirstMovePlayer)Enum.Parse(typeof(FirstMovePlayer), firstMoveString, true);
            GameCore.Initialized += () => {
                SetCurrentPlayer(GameCore.GetCurrentPlayer());
            };
            GameCore.ScoreChanged += ChangeScore;
            GameCore.FieldFilled += GameOver;
            GameCore.SwapPlayersEvent += SetCurrentPlayer;
            GameCore.Init(ref fieldGrid, fieldSize, first);
            GameCore.GenerateField();
            linesWidth = GameCore.GetLinesWidth();
        }

        private void fieldGrid_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e) {
            manipulationHandler?.Clear();
            manipulationHandler = new ManipulationHandler(fieldSize, linesWidth, ref fieldGrid);
            manipulationHandler.ManipulationStarted(e.ManipulationOrigin);
        }

        private void fieldGrid_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e) {
            manipulationHandler.ManipulationDelta(e.DeltaManipulation.Translation);
        }

        private void fieldGrid_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e) {
            manipulationHandler.ManipulationCompleted(e.TotalManipulation.Translation);
        }
    }
}