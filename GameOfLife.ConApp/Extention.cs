namespace GameOfLife.ConApp;

public static partial class Extention
{
    extension(bool[][] current)
    {
        public bool IsEqual(bool[][]? compare)
        {
            if (compare is null || current.Length != compare.Length) return false;
            for (var row = 0; row < current.Length; row++)
            {
                if (!current[row].SequenceEqual(compare[row])) return false;
            }
            return true;
        }

        public int TotalLife() =>
            current.Sum(row => row.Sum(static cell => cell ? 1 : 0));
    }
}
