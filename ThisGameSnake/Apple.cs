using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameSnake;
using System.Windows;

namespace GameSnake
{
    public class Apple
    {
        private const int MapSize = 20;
        private readonly Random random = new();
        public Point Position { get; private set; }

        public void SetPosition(Point position)
        {
            Position = position;
        }

        public void Spawn(Snake snake)
        {
            do
            {
                Position = new Point(random.Next(0, MapSize), random.Next(0, MapSize));
            }
            while (snake.Contains(Position));
        }
    }
}
