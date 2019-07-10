using System;
using System.Collections.Generic;
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
    }
}
