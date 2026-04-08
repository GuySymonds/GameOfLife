using System;
using System.Threading;
using System.Threading.Tasks;
using GameOfLife.ConApp;
using Spectre.Console;

AnsiConsole.Write(new FigletText("Game of Life").Centered().Color(Color.Green));
AnsiConsole.WriteLine();

var width = 80;
var height = 30;
var sleepMs = 100;

IGame game = new Game();
var current = await game.GetNewGameAsync(new GameOfLife.Common.Models.NewGameModel(width, height));

var changing = true;
int cycles = 0;
byte[][]? last = null;
byte[][]? secondLast = null;

var liveColor = Color.Green;

while (changing)
{
    cycles++;
    current = await game.GetNextGameStateAsync(current.GameId);

    AnsiConsole.Clear();

    var panel = BuildGamePanel(current.Cells, cycles, liveColor);
    AnsiConsole.Write(panel);

    if (current.Cells.IsEqual(last))
    {
        changing = false;
        AnsiConsole.MarkupLine("[yellow]Steady state reached — this generation matches the last.[/]");
    }
    else if (current.Cells.IsEqual(secondLast))
    {
        changing = false;
        AnsiConsole.MarkupLine("[yellow]Oscillator detected — this generation matches two cycles ago.[/]");
    }
    else
    {
        Thread.Sleep(sleepMs);
        secondLast = last;
        last = current.Cells;
    }
}

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

    for (var col = 0; col < columns; col++)
    {
        var line = new Markup(BuildRow(cells, rows, col, liveChar, liveColor));
        grid.AddRow(line);
    }

    return new Panel(grid)
        .Header($"[bold]Cycle {cycle,4}[/]  [green]Alive: {totalLife,5}[/]")
        .BorderStyle(Style.Parse("blue"))
        .Padding(0, 0);
}

static string BuildRow(byte[][] cells, int rows, int col, char liveChar, Color liveColor)
{
    var sb = new System.Text.StringBuilder();
    for (var row = 0; row < rows; row++)
    {
        if (cells[row][col] == 1)
            sb.Append($"[{liveColor}]{liveChar}[/]");
        else
            sb.Append(' ');
    }
    return sb.ToString();
}
