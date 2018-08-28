using System;

namespace GameOfLife
{
    public class Life
    {
        public bool[,] Past { get; private set; }

        public bool[,] Present
        {
            get
            {
                if (_present == null)
                    _present = GenerateSeed();
                return _present;
            }
            private set
            {
                _present = value;
            }
        }

        private bool[,] _present;

        private readonly int _rows;
        private readonly int _columns;
        readonly Random _rand = new Random();

        public Life(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
        }

        public void GenerateSeed(bool[,] seed = null)
        {
            if(seed == null)
            {
                seed = GenerateSeed();
            }
            Present = seed;
            Past = null;
        }

        private bool[,] GenerateSeed()
        {
            bool[,] current = new bool[_rows, _columns];
            for (int column = 0; column < _columns; column++)
            {
                for (int row = 0; row < _rows; row++)
                {
                    current[row, column] = _rand.NextDouble() < 0.2 ? true : false;
                }
            }

            return current;
        }

        public void Tick()
        {
            Past = Present;
            bool[,] future = new bool[_rows, _columns];

            for (int column = 0; column < _columns; column++)
            {
                for (int row = 0; row < _rows; row++)
                {
                    future[row, column] = Present.Law(row, column);
                }
            }

            Present = future;
            Print();
        }

        public void Print()
        {
            Console.Clear();
            for (int column = 0; column < _columns; column++)
            {
                for (int row = 0; row < _rows; row++)
                {
                    Console.Write($"{(Present[row, column] ? "#" : " ")}");
                }
                Console.WriteLine();
            }
        }
    }
}
