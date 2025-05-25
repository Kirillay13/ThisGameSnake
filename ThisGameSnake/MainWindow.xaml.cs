using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameSnake
{
    public partial class MainWindow : Window
    {
        private const int TileSize = 20;

        private readonly GameLogic gameLogic;
        private DispatcherTimer gameTimer;
        private ImageBrush foodBrush;
        private ImageBrush mapBrush;

        public MainWindow()
        {
            InitializeComponent();
            LoadResources();

            gameLogic = new GameLogic();
            gameLogic.GameOverEvent += OnGameOver;
            gameLogic.RedrawNeeded += DrawGame;

            InitializeTimer();
        }

        private void LoadResources()
        {
            try
            {
                foodBrush = new ImageBrush(new BitmapImage(new Uri("E:\\PROGRAMS C#\\ThisGameSnake\\ThisGameSnake\\Resource\\apple.png", UriKind.Relative)));
                mapBrush = new ImageBrush(new BitmapImage(new Uri("E:\\PROGRAMS C#\\ThisGameSnake\\ThisGameSnake\\Resource\\map.jpg", UriKind.Relative)));
                GameCanvas.Background = mapBrush;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ресурсов: {ex.Message}");
            }
        }

        private void InitializeTimer()
        {
            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(125)
            };
            gameTimer.Tick += (s, e) => gameLogic.UpdateGame();
        }

        private void DrawGame()
        {
            GameCanvas.Children.Clear();

            // Отрисовка змейки
            foreach (var position in gameLogic.Snake.Body)
            {
                Rectangle segment = new Rectangle
                {
                    Width = TileSize,
                    Height = TileSize,
                    Fill = Brushes.Green
                };
                Canvas.SetLeft(segment, position.X * TileSize);
                Canvas.SetTop(segment, position.Y * TileSize);
                GameCanvas.Children.Add(segment);
            }

            // Отрисовка яблока
            Rectangle food = new Rectangle
            {
                Width = TileSize,
                Height = TileSize,
                Fill = foodBrush
            };
            var foodPos = gameLogic.Apple.Position;
            Canvas.SetLeft(food, foodPos.X * TileSize);
            Canvas.SetTop(food, foodPos.Y * TileSize);
            GameCanvas.Children.Add(food);

            // Обновление счёта
            CurrentScoreText.Text = gameLogic.Score.ToString();
            HighScoreText.Text = gameLogic.HighScore.ToString();
        }

        private void OnGameOver()
        {
            gameTimer.Stop();
            MessageBox.Show($"Игра окончена! Счёт: {gameLogic.Score}");
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.W when gameLogic.Snake.Direction != GameLogic.Direction.Down:
                    gameLogic.Snake.Direction = GameLogic.Direction.Up;
                    break;
                case Key.S when gameLogic.Snake.Direction != GameLogic.Direction.Up:
                    gameLogic.Snake.Direction = GameLogic.Direction.Down;
                    break;
                case Key.A when gameLogic.Snake.Direction != GameLogic.Direction.Right:
                    gameLogic.Snake.Direction = GameLogic.Direction.Left;
                    break;
                case Key.D when gameLogic.Snake.Direction != GameLogic.Direction.Left:
                    gameLogic.Snake.Direction = GameLogic.Direction.Right;
                    break;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            gameLogic.StartNewGame();
            gameTimer.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameTimer.IsEnabled)
                gameTimer.Stop();
            else
                gameTimer.Start();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            gameLogic.SaveGame();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            gameLogic.LoadGame();
            DrawGame();
            gameTimer.Start();
        }
    }
}
