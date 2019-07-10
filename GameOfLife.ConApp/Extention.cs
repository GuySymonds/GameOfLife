using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameOfLife.ConApp
{
    public static partial class Extention
    {
        public static void Print(this byte[,] state)
        {
            var rows = state.GetLength(0);
            var columns = state.GetLength(1);
            StringBuilder sb = new StringBuilder();
            
            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    sb.Append(state[row,column] == 1 ? "#" : " ");
                }
                sb.AppendLine();
            }
            Console.Clear();
            Console.Write(sb);
        }

        public static bool IsEqual(this byte[,] current, byte[,] compare)
        {
            return current?.Rank == compare?.Rank &&
                Enumerable.Range(0, current.Rank).All(dimension => current.GetLength(dimension) == compare.GetLength(dimension)) &&
                current.Cast<byte>().SequenceEqual(compare.Cast<byte>());
        }
    }
}
