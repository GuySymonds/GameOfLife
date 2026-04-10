using GameOfLife.Engine;

namespace GameOfLife.Tests;

public class GameEngineTests
{
    private readonly GameEngine _engine = new();

    // ── GenerateSeed ────────────────────────────────────────────────────────

    [Theory]
    [InlineData(5, 10)]
    [InlineData(18, 40)]
    [InlineData(1, 1)]
    public void GenerateSeed_Returns_CorrectShape(int rows, int columns)
    {
        var seed = _engine.GenerateSeed(rows, columns);

        Assert.Equal(rows, seed.Length);
        for (var r = 0; r < rows; r++)
            Assert.Equal(columns, seed[r].Length);
    }

    // ── GetNextState — still life (block) ───────────────────────────────────

    /// <summary>
    /// A 2×2 block of live cells in a larger grid is a still-life:
    /// every live cell has exactly 3 live neighbours and every dead cell
    /// bordering the block has fewer than 3, so nothing changes.
    /// </summary>
    [Fact]
    public void GetNextState_Block_IsStillLife()
    {
        var grid = new bool[][]
        {
            [false, false, false, false],
            [false, true,  true,  false],
            [false, true,  true,  false],
            [false, false, false, false],
        };

        var next = _engine.GetNextState(grid);

        // The four block cells must all survive
        Assert.True(next[1][1]);
        Assert.True(next[1][2]);
        Assert.True(next[2][1]);
        Assert.True(next[2][2]);

        // No new cells should come alive in the corners or edges
        Assert.False(next[0][0]);
        Assert.False(next[0][3]);
        Assert.False(next[3][0]);
        Assert.False(next[3][3]);
    }

    // ── GetNextState — oscillator (blinker) ─────────────────────────────────

    /// <summary>
    /// A horizontal blinker (period-2 oscillator):
    /// Gen 0: row 2, columns 1-2-3 are alive.
    /// Gen 1: column 2, rows 1-2-3 are alive.
    /// Gen 2: back to gen 0.
    /// </summary>
    [Fact]
    public void GetNextState_Blinker_OscillatesHorizontalToVertical()
    {
        var gen0 = new bool[][]
        {
            [false, false, false, false, false],
            [false, false, false, false, false],
            [false, true,  true,  true,  false],
            [false, false, false, false, false],
            [false, false, false, false, false],
        };

        var gen1 = _engine.GetNextState(gen0);

        // Vertical bar should be alive
        Assert.True(gen1[1][2]);
        Assert.True(gen1[2][2]);
        Assert.True(gen1[3][2]);

        // Original horizontal arms should be dead
        Assert.False(gen1[2][1]);
        Assert.False(gen1[2][3]);
    }

    [Fact]
    public void GetNextState_Blinker_OscillatesVerticalToHorizontal()
    {
        var gen1 = new bool[][]
        {
            [false, false, false, false, false],
            [false, false, true,  false, false],
            [false, false, true,  false, false],
            [false, false, true,  false, false],
            [false, false, false, false, false],
        };

        var gen2 = _engine.GetNextState(gen1);

        // Horizontal bar should be alive
        Assert.True(gen2[2][1]);
        Assert.True(gen2[2][2]);
        Assert.True(gen2[2][3]);

        // Original vertical arms should be dead
        Assert.False(gen2[1][2]);
        Assert.False(gen2[3][2]);
    }

    // ── GetNextState — underpopulation / overpopulation ─────────────────────

    [Fact]
    public void GetNextState_LiveCellWithFewerThanTwoNeighbours_Dies()
    {
        // Single live cell in a 3×3 grid — has 0 neighbours
        var grid = new bool[][]
        {
            [false, false, false],
            [false, true,  false],
            [false, false, false],
        };

        var next = _engine.GetNextState(grid);

        Assert.False(next[1][1]);
    }

    [Fact]
    public void GetNextState_DeadCellWithExactlyThreeNeighbours_BecomesAlive()
    {
        // Three live cells surround the centre dead cell
        var grid = new bool[][]
        {
            [false, false, false],
            [true,  false, true ],
            [false, true,  false],
        };

        var next = _engine.GetNextState(grid);

        Assert.True(next[1][1]);
    }
}
