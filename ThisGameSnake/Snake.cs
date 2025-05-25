using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameSnake;
using System.Collections.Generic;
using System.Windows;

namespace GameSnake
{
    public class Snake
    {
        public List<Point> Body { get; set; } = new();

        public GameLogic.Direction Direction { get; set; } = GameLogic.Direction.Right;

        public Snake()
        {
            Reset();
        }

        public void Reset()
        {
            Body.Clear();
            Body.Add(new Point(5, 5));
            Body.Add(new Point(4, 5));
            Body.Add(new Point(3, 5));
            Direction = GameLogic.Direction.Right;
        }

        public void Move(Point newHead, bool grow)
        {
            Body.Insert(0, newHead);
            if (!grow)
            {
                Body.RemoveAt(Body.Count - 1);
            }
        }

        public Point GetHead() => Body[0];

        public bool Contains(Point point) => Body.Contains(point);
    }
}
