namespace GameOfLife
{

    public static partial class Extention
    {
        public static bool Law(this bool[,] present, int row, int column)
        {
            var cell = present[row, column];
            var maxRow = present.GetLength(0) - 1;
            var maxColumn = present.GetLength(1) - 1;

            var northRow = row - 1 == -1 ? maxRow : row - 1;
            var southRow = row + 1 > maxRow ? 0 : row + 1;
            var eastColumn = column + 1 > maxColumn ? 0 : column + 1;
            var westColumn = column - 1 == -1 ? maxColumn : column - 1;

            var neighbors = (present[northRow, westColumn] ? 1 : 0) +
                (present[northRow, column] ? 1 : 0) +
                (present[northRow, eastColumn] ? 1 : 0) +
                (present[row, eastColumn] ? 1 : 0) +
                (present[southRow, eastColumn] ? 1 : 0) +
                (present[southRow, column] ? 1 : 0) +
                (present[southRow, westColumn] ? 1 : 0) +
                (present[row, westColumn] ? 1 : 0);

            if (cell && neighbors < 2)
            {
                //Any live cell with fewer than two live neighbors dies, as if by under population
                cell = false;
            }
            else if (cell && neighbors > 3)
            {
                //Any live cell with more than three live neighbors dies, as if by overpopulation
                cell = false;
            }
            else if (!cell && neighbors == 3)
            {
                cell = true;
            }

            return cell;
        }
    }
}
