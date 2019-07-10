using System;
using System.Threading;

namespace GameOfLife.ConApp
{
    class Program
    {
        static Guid id = Guid.Parse("e6829659-e497-461d-8313-2993b9a3d9e8");
        static void Main(string[] args)
        {
            var sleeper = 0;
            Console.WriteLine("Game Of Life!");
            var changing = true;
            var game = new Game();
            var current = game.GetNewGame(new Common.Models.NewGameModel(Console.WindowWidth, Console.WindowHeight));
            current.Cells.Print();
            int cycles = 0;
            byte[,] last = null;
            byte[,] secondLast = null;

            while (changing)
            {
                cycles++;
                current = game.GetNextGameState(current.GameId);
                if(current.Cells.IsEqual(last))
                {
                    changing = false;
                    Console.WriteLine("This is the same as the last iteration");
                }
                else if (current.Cells.IsEqual(secondLast))
                {
                    changing = false;
                    Console.WriteLine("This is the same as it was two iterations ago");
                }
                else
                {
                    current.Cells.Print();
                    Thread.Sleep(sleeper);
                    Console.WriteLine("Total Cycles: {0}", cycles);
                    secondLast = last;
                    last = current.Cells;
                }
            }
            Console.WriteLine("Done!");
            Console.Read();
        }
    }
}
