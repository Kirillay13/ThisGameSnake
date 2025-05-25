using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using static GameSnake.GameLogic;

namespace GameSnake
{
    public class GameState
    {
        public List<Point> SnakePositions { get; set; }
        public Point FoodPosition { get; set; }
        public GameLogic.Direction CurrentDirection { get; set; }
        public int Score { get; set; }
    }
}
