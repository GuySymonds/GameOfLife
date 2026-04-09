using GameOfLife.ConApp;
using Spectre.Console;

AnsiConsole.Write(new FigletText("Game of Life").Centered().Color(Color.Green));
AnsiConsole.WriteLine();

var width = 80;
var height = 30;
var sleepMs = 100;

IGame game = new Game();
var current = await game.GetNewGameAsync(new GameOfLife.Common.Models.NewGameModel(width, height));

int cycles = 0;
byte[][]? last = null;
byte[][]? secondLast = null;
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

static Panel BuildGamePanel(byte[][] cells, int cycle, Color liveColor)
{
    var rows = cells.Length;
    var columns = cells[0].Length;
    var totalLife = cells.TotalLife();
    var liveChar = cycle % 2 == 0 ? '█' : '▓';

    var grid = new Table().Border(TableBorder.None).HideHeaders();
    grid.AddColumn(new TableColumn("board").NoWrap());

    // Each column index maps to one visual row in the output
    for (var y = 0; y < columns; y++)
    {
        var line = new Markup(BuildRow(cells, rows, y, liveChar, liveColor));
        grid.AddRow(line);
    }

    return new Panel(grid)
        .Header($"[bold]Cycle {cycle,4}[/]  [green]Alive: {totalLife,5}[/]")
        .BorderStyle(Style.Parse("blue"))
        .Padding(0, 0);
}

static string BuildRow(byte[][] cells, int rows, int y, char liveChar, Color liveColor)
{
    var sb = new System.Text.StringBuilder();
    for (var x = 0; x < rows; x++)
    {
        if (cells[x][y] == 1)
            sb.Append($"[{liveColor}]{liveChar}[/]");
        else
            sb.Append(' ');
    }
    return sb.ToString();
}
