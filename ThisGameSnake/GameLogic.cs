using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Windows;


namespace GameSnake
{
    public class GameLogic
    {
        private const int MapSize = 20;

        public Snake Snake { get; private set; } = new();
        public Apple Apple { get; private set; } = new();
        public int Score { get; private set; }
        public int HighScore { get; private set; }

        public event Action GameOverEvent;
        public event Action RedrawNeeded;

        public GameLogic()
        {
            if (File.Exists("highscore.txt"))
                HighScore = int.Parse(File.ReadAllText("highscore.txt"));
        }

        public void StartNewGame()
        {
            Snake.Reset();
            Score = 0;
            Apple.Spawn(Snake);
            RedrawNeeded?.Invoke();
        }

        public void UpdateGame()
        {
            Point head = Snake.GetHead();
            Point newHead = head;

            switch (Snake.Direction)
            {
                case Direction.Up: newHead.Y--; break;
                case Direction.Down: newHead.Y++; break;
                case Direction.Left: newHead.X--; break;
                case Direction.Right: newHead.X++; break;
            }

            if (newHead.X < 0 || newHead.X >= MapSize ||
                newHead.Y < 0 || newHead.Y >= MapSize ||
                Snake.Contains(newHead))
            {
                GameOverEvent?.Invoke();
                return;
            }

            bool grow = newHead == Apple.Position;
            Snake.Move(newHead, grow);

            if (grow)
            {
                Score++;
                if (Score > HighScore)
                {
                    HighScore = Score;
                    File.WriteAllText("highscore.txt", HighScore.ToString());
                }
                Apple.Spawn(Snake);
            }

            RedrawNeeded?.Invoke();
        }

        public void SaveGame()
        {
            var gameState = new GameState
            {
                SnakePositions = Snake.Body,
                FoodPosition = Apple.Position,
                CurrentDirection = Snake.Direction,
                Score = Score
            };
            string json = JsonSerializer.Serialize(gameState);
            File.WriteAllText("save.json", json);
        }

        public void LoadGame()
        {
            if (!File.Exists("save.json")) return;

            string json = File.ReadAllText("save.json");
            var gameState = JsonSerializer.Deserialize<GameState>(json);

            if (gameState == null) return;

            Snake.Body = gameState.SnakePositions;
            Snake.Direction = gameState.CurrentDirection;
            Apple.SetPosition(gameState.FoodPosition);
            Score = gameState.Score;
            RedrawNeeded?.Invoke();
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
