namespace GameOfLife.Engine;

public class GameEngine
{
    private static readonly Lazy<Random> _rand = new(() => new Random());

    public Random Rand => _rand.Value;

    public bool[][] GenerateSeed(int rows, int columns)
    {
        var seed = new bool[rows][];
        for (var row = 0; row < rows; row++)
        {
            seed[row] = new bool[columns];
            for (var column = 0; column < columns; column++)
            {
                seed[row][column] = _rand.Value.NextDouble() < 0.2;
            }
        }
        return seed;
    }

    public bool[][] GetNextState(bool[][] game)
    {
        var rows = game.Length;
        var columns = game[0].Length;
        var future = new bool[rows][];

        for (var row = 0; row < rows; row++)
        {
            future[row] = new bool[columns];
            for (var column = 0; column < columns; column++)
            {
                future[row][column] = ApplyLaw(game, row, column);
            }
        }

        return future;
    }

    private static bool ApplyLaw(bool[][] present, int row, int column)
    {
        var cell = present[row][column];
        var maxRow = present.Length - 1;
        var maxColumn = present[0].Length - 1;

        var northRow = row - 1 == -1 ? maxRow : row - 1;
        var southRow = row + 1 > maxRow ? 0 : row + 1;
        var eastColumn = column + 1 > maxColumn ? 0 : column + 1;
        var westColumn = column - 1 == -1 ? maxColumn : column - 1;

        var neighbors = (present[northRow][westColumn] ? 1 : 0) +
                        (present[northRow][column] ? 1 : 0) +
                        (present[northRow][eastColumn] ? 1 : 0) +
                        (present[row][eastColumn] ? 1 : 0) +
                        (present[southRow][eastColumn] ? 1 : 0) +
                        (present[southRow][column] ? 1 : 0) +
                        (present[southRow][westColumn] ? 1 : 0) +
                        (present[row][westColumn] ? 1 : 0);

        if (cell && neighbors < 2)
        {
            return false;
        }
        else if (cell && neighbors > 3)
        {
            return false;
        }
        else if (!cell && neighbors == 3)
        {
            return true;
        }

        return cell;
    }
}
