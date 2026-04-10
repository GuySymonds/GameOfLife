using GameOfLife.ConApp;
using Spectre.Console;

AnsiConsole.Write(new FigletText("Game of Life").Centered().Color(Color.Green));
AnsiConsole.WriteLine();

var (width, height) = GetBoardSize();
var sleepMs = 100;

IGame game = new Game();
var current = await game.GetNewGameAsync(new GameOfLife.Common.Models.NewGameModel(width, height));

int cycles = 0;
bool[][]? last = null;
bool[][]? secondLast = null;
string stopReason = string.Empty;

var liveColor = Color.Green;

await AnsiConsole.Live(BuildGamePanel(current.Cells, cycles, liveColor))
    .AutoClear(false)
    .Overflow(VerticalOverflow.Ellipsis)
    .StartAsync(async ctx =>
    {
        while (string.IsNullOrEmpty(stopReason))
        {
            cycles++;
            current = await game.GetNextGameStateAsync(current.GameId);

            ctx.UpdateTarget(BuildGamePanel(current.Cells, cycles, liveColor));
            ctx.Refresh();

            if (current.Cells.IsEqual(last))
                stopReason = "[yellow]Steady state reached — this generation matches the last.[/]";
            else if (current.Cells.IsEqual(secondLast))
                stopReason = "[yellow]Oscillator detected — this generation matches two cycles ago.[/]";
            else
            {
                Thread.Sleep(sleepMs);
                secondLast = last;
                last = current.Cells;
            }
        }
    });

AnsiConsole.MarkupLine(stopReason);
AnsiConsole.MarkupLine("[bold green]Simulation complete![/]");
AnsiConsole.MarkupLine($"[grey]Total cycles: {cycles}[/]");
Console.Read();

static (int Width, int Height) GetBoardSize()
{
    var availableWidth = Math.Max(20, Console.WindowWidth);
    var availableHeight = Math.Max(12, Console.WindowHeight);

    var isLandscape = availableWidth >= availableHeight;
    var widthUsage = isLandscape ? 0.9 : 0.65;
    var heightUsage = isLandscape ? 0.65 : 0.9;

    var width = Math.Max(10, (int)(availableWidth * widthUsage));
    var height = Math.Max(10, (int)(availableHeight * heightUsage));

    if (isLandscape && width <= height)
        width = height + 1;

    if (!isLandscape && height <= width)
        height = width + 1;

    return (width, height);
}

static Panel BuildGamePanel(bool[][] cells, int cycle, Color liveColor)
{
    var rows = cells.Length;
    var columns = cells[0].Length;
    var totalLife = cells.TotalLife();
    var liveChar = cycle % 2 == 0 ? '█' : '▓';

    var grid = new Table().Border(TableBorder.None).HideHeaders();
    grid.AddColumn(new TableColumn("board").NoWrap());

    // Each row maps to one visual row in the output
    for (var r = 0; r < rows; r++)
    {
        var line = new Markup(BuildRow(cells[r], columns, liveChar, liveColor));
        grid.AddRow(line);
    }

    return new Panel(grid)
        .Header($"[bold]Cycle {cycle,4}[/]  [green]Alive: {totalLife,5}[/]")
        .BorderStyle(Style.Parse("blue"))
        .Padding(0, 0);
}

static string BuildRow(bool[] row, int columns, char liveChar, Color liveColor)
{
    var sb = new System.Text.StringBuilder();
    for (var x = 0; x < columns; x++)
    {
        if (row[x])
            sb.Append($"[{liveColor}]{liveChar}[/]");
        else
            sb.Append(' ');
    }
    return sb.ToString();
}
