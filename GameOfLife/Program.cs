using System;
using System.Threading;

namespace GameOfLife
{
    class Program
    {
        static void Main(string[] args)
        {
            var seconds = 20;
            Console.WriteLine("Game Of Life!");
            var life = new Life(Console.WindowWidth, Console.WindowHeight);
            life.Print();

            while (true)
            {
                life.Tick();
                Thread.Sleep(500);
            }
        }
    }
}
