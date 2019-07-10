using System;
using System.Threading;

namespace GameOfLife.ConApp
{
    class Program
    {
        static Guid id = Guid.Parse("e6829659-e497-461d-8313-2993b9a3d9e8");
        static void Main(string[] args)
        {
            var sleeper = 500;
            Console.WriteLine("Game Of Life!");
            
            var game = new Game();
            var current = game.GetNewGame(new Common.Models.NewGameModel(Console.WindowWidth, Console.WindowHeight));
            current.Cells.Print();

            while (true)
            {
                game.GetNextGameState(current.GameId).Cells.Print();
                Thread.Sleep(sleeper);
            }
        }
    }
}
