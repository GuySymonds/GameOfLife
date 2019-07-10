using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife.Engine
{
    public class GameEngine
    {
        private static readonly Lazy<Random> _rand = new Lazy<Random>(()=>new Random());

        public Random Rand => _rand.Value;

        public byte[,] GenerateSeed(int rows, int columns)
        {
            var seed = new byte[rows, columns];
            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    seed[row, column] = _rand.Value.NextDouble() < 0.2 ? (byte)1 : (byte)0;
                }
            }

            return seed;
        }

        public byte[,] GetNextState(byte[,] game)
        {
            var rows = game.GetLength(0);
            var columns = game.GetLength(1);

            var future = new byte[rows, columns];

            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    future[row, column] = ApplyLaw(game, row, column);
                }
            }

            return future;
        }

        private static byte ApplyLaw(byte[,] present, int row, int column)
        {
            var cell = present[row, column];
            var maxRow = present.GetLength(0) - 1;
            var maxColumn = present.GetLength(1) - 1;

            var northRow = row - 1 == -1 ? maxRow : row - 1;
            var southRow = row + 1 > maxRow ? 0 : row + 1;
            var eastColumn = column + 1 > maxColumn ? 0 : column + 1;
            var westColumn = column - 1 == -1 ? maxColumn : column - 1;

            var neighbors = present[northRow, westColumn] +
                            present[northRow, column] +
                            present[northRow, eastColumn] +
                            present[row, eastColumn] +
                            present[southRow, eastColumn]+
                            present[southRow, column] +
                            present[southRow, westColumn] +
                            present[row, westColumn];

            if (cell == 1 && neighbors < 2)
            {
                //Any live cell with fewer than two live neighbors dies, as if by under population
                cell = 0;
            }
            else if (cell == 1 && neighbors > 3)
            {
                //Any live cell with more than three live neighbors dies, as if by overpopulation
                cell = 0;
            }
            else if (cell == 0 && neighbors == 3)
            {
                //Any dead cell with exactly 3 live neighbors lives, as if by reproduction
                cell = 1;
            }

            return cell;
        }
    }
}
