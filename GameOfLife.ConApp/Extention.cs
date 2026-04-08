using System;
using System.Linq;
using System.Text;

namespace GameOfLife.ConApp;

public static partial class Extention
{
    public static bool IsEqual(this byte[][] current, byte[][]? compare)
    {
        if (compare is null || current.Length != compare.Length) return false;
        for (var row = 0; row < current.Length; row++)
        {
            if (!current[row].SequenceEqual(compare[row])) return false;
        }
        return true;
    }

    public static int TotalLife(this byte[][] current) =>
        current.Sum(row => row.Sum(cell => (int)cell));
}
