using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Text.Json;

namespace GameSnake
{
    public partial class MainWindow : Window
    {
        private const int TileSize = 20;
        private const int MapSize = 20;

        private List<Point> snakePositions = new List<Point>();
        private Point foodPosition;
        private Direction currentDirection = Direction.Right;
        private DispatcherTimer gameTimer;
        private Random random = new Random();
        private int score = 0;
        private int highScore = 0;
        private bool isGameRunning = false;

        private ImageBrush foodBrush;
        private ImageBrush mapBrush;

        public MainWindow()
        {
            InitializeComponent();
            LoadResources();
            InitializeGame();
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

        private void InitializeGame()
        {
            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            gameTimer.Tick += GameLoop;

            if (File.Exists("highscore.txt"))
            {
                highScore = int.Parse(File.ReadAllText("highscore.txt"));
                HighScoreText.Text = highScore.ToString();
            }

        }

        private void StartNewGame()
        {
            GameCanvas.Children.Clear();
            snakePositions.Clear();
            score = 0;
            CurrentScoreText.Text = "0";
            currentDirection = Direction.Right;

            // Начальная позиция змейки
            snakePositions.Add(new Point(5, 5));
            snakePositions.Add(new Point(4, 5));
            snakePositions.Add(new Point(3, 5));

            DrawSnake();
            SpawnFood();

            isGameRunning = true;
            gameTimer.Start();
        }

        private void DrawSnake()
        {
            GameCanvas.Children.Clear();

            // Отрисовка змейки
            foreach (Point position in snakePositions)
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

            // Отрисовка еды
            Rectangle food = new Rectangle
            {
                Width = TileSize,
                Height = TileSize,
                Fill = foodBrush
            };

            Canvas.SetLeft(food, foodPosition.X * TileSize);
            Canvas.SetTop(food, foodPosition.Y * TileSize);
            GameCanvas.Children.Add(food);
        }

        private void SpawnFood()
        {
            do
            {
                foodPosition = new Point(random.Next(0, MapSize), random.Next(0, MapSize));
            }
            while (snakePositions.Contains(foodPosition));
        }

        private void GameLoop(object sender, EventArgs e)
        {
            Point head = snakePositions[0];
            Point newHead = head;

            switch (currentDirection)
            {
                case Direction.Up:
                    newHead.Y--;
                    break;
                case Direction.Down:
                    newHead.Y++;
                    break;
                case Direction.Left:
                    newHead.X--;
                    break;
                case Direction.Right:
                    newHead.X++;
                    break;
            }

            // Проверка столкновений
            if (newHead.X < 0 || newHead.X >= MapSize ||
                newHead.Y < 0 || newHead.Y >= MapSize ||
                snakePositions.Contains(newHead))
            {
                GameOver();
                return;
            }

            snakePositions.Insert(0, newHead);

            if (newHead == foodPosition)
            {
                score++;
                CurrentScoreText.Text = score.ToString();
                if (score > highScore)
                {
                    highScore = score;
                    HighScoreText.Text = highScore.ToString();
                    File.WriteAllText("highscore.txt", highScore.ToString());
                }
                SpawnFood();
            }
            else
            {
                snakePositions.RemoveAt(snakePositions.Count - 1);
            }

            DrawSnake();
        }

        private void GameOver()
        {
            gameTimer.Stop();
            isGameRunning = false;
            MessageBox.Show($"Игра окончена! Счет: {score}");
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!isGameRunning) return;

            switch (e.Key)
            {
                case Key.Up when currentDirection != Direction.Down:
                    currentDirection = Direction.Up;
                    break;
                case Key.Down when currentDirection != Direction.Up:
                    currentDirection = Direction.Down;
                    break;
                case Key.Left when currentDirection != Direction.Right:
                    currentDirection = Direction.Left;
                    break;
                case Key.Right when currentDirection != Direction.Left:
                    currentDirection = Direction.Right;
                    break;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isGameRunning)
            {
                if (gameTimer.IsEnabled)
                    gameTimer.Stop();
                else
                    gameTimer.Start();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isGameRunning) return;

            var gameState = new GameState
            {
                SnakePositions = snakePositions,
                FoodPosition = foodPosition,
                CurrentDirection = currentDirection,
                Score = score
            };

            string json = JsonSerializer.Serialize(gameState);
            File.WriteAllText("save.json", json);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("save.json")) return;

            string json = File.ReadAllText("save.json");
            var gameState = JsonSerializer.Deserialize<GameState>(json);

            if (gameState == null) return;

            gameTimer.Stop();
            snakePositions = gameState.SnakePositions;
            foodPosition = gameState.FoodPosition;
            currentDirection = gameState.CurrentDirection;
            score = gameState.Score;
            CurrentScoreText.Text = score.ToString();

            DrawSnake();
            isGameRunning = true;
            gameTimer.Start();
        }
    }

    public class GameState
    {
        public List<Point> SnakePositions { get; set; }
        public Point FoodPosition { get; set; }
        public Direction CurrentDirection { get; set; }
        public int Score { get; set; }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
