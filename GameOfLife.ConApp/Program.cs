using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife.ConApp
{
    class Program
    {
        static Guid id = Guid.Parse("e6829659-e497-461d-8313-2993b9a3d9e8");

        private static int _width = Console.WindowWidth;
        private static int _height = Console.WindowHeight;

        static async Task Main()
        {
            _width = 200;
            _height = 40;
            Thread.Sleep(1000);
            var sleeper = 100;
            Console.WriteLine("Game Of Life!");
            var changing = true;
            IGame game = new Game();
            var current = await game.GetNewGameAsync(new Common.Models.NewGameModel(_width, _height));
            
            Console.Write(current.Cells.ToString('#'));
            int cycles = 0;
            byte[,] last = null;
            byte[,] secondLast = null;

            while (changing)
            {
                cycles++;
                current = await game.GetNextGameStateAsync(current.GameId);
                Console.Clear();
                if (current.Cells.IsEqual(last))
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
                    if(cycles % 2 == 0)
                        Console.Write(current.Cells.ToString('#'));
                    else
                        Console.Write(current.Cells.ToString('='));

                    Thread.Sleep(sleeper);
                    Console.WriteLine("Total Cycles: {0}", cycles);
                    secondLast = last;
                    last = current.Cells;
                }
                    Console.WriteLine("Galactic Life: {0}", current.Cells.TotalLife());

                /*if (cycles % 9 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                } 
                else if (cycles % 6 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                } 
                else if (cycles % 3 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }*/
            }
            Console.WriteLine("Done!");
            Console.Read();
        }
    }
}
