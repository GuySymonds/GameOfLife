using GameOfLife.Api.Services;
using GameOfLife.Common.Models;
using GameOfLife.Engine;

namespace GameOfLife.Tests;

public class GameServiceTests
{
    // Each test gets its own service instance, but note that GameService uses a
    // static data store so games created in one test persist across the suite.
    // Tests are written to be independent: they only assert on the game they create.
    private readonly GameService _service = new(new GameEngine());

    // ── NewGame — dimension mapping ──────────────────────────────────────────

    /// <summary>
    /// NewGameModel.Width represents columns and Height represents rows.
    /// GameService must pass (Height, Width) to the engine so that the resulting
    /// grid has the correct orientation: Cells[row][column].
    /// </summary>
    [Theory]
    [InlineData(10, 20)]
    [InlineData(40, 18)]
    [InlineData(1, 1)]
    [InlineData(100, 50)]
    public void NewGame_Maps_WidthToColumns_And_HeightToRows(int width, int height)
    {
        var model = new NewGameModel(width, height);

        var game = _service.NewGame(model);

        Assert.Equal(height, game.Cells.Length);           // outer index = rows = height
        Assert.Equal(width, game.Cells[0].Length);          // inner index = columns = width
    }

    [Fact]
    public void NewGame_AssignsNewGameId()
    {
        var game = _service.NewGame(new NewGameModel(5, 5));

        Assert.NotEqual(Guid.Empty, game.GameId);
    }

    [Fact]
    public void NewGame_TwoGames_HaveDifferentIds()
    {
        var game1 = _service.NewGame(new NewGameModel(5, 5));
        var game2 = _service.NewGame(new NewGameModel(5, 5));

        Assert.NotEqual(game1.GameId, game2.GameId);
    }

    // ── CurrentState ────────────────────────────────────────────────────────

    [Fact]
    public void CurrentState_ReturnsGame_WhenIdExists()
    {
        var created = _service.NewGame(new NewGameModel(5, 5));

        var retrieved = _service.CurrentState(created.GameId);

        Assert.Equal(created.GameId, retrieved.GameId);
    }

    [Fact]
    public void CurrentState_Throws_WhenIdNotFound()
    {
        Assert.Throws<KeyNotFoundException>(() => _service.CurrentState(Guid.NewGuid()));
    }

    // ── NextState ────────────────────────────────────────────────────────────

    [Fact]
    public void NextState_ReturnsSameGameId()
    {
        var game = _service.NewGame(new NewGameModel(10, 10));
        var originalId = game.GameId;

        var next = _service.NextState(game);

        Assert.Equal(originalId, next.GameId);
    }

    [Fact]
    public void NextState_RetainsGridDimensions()
    {
        var game = _service.NewGame(new NewGameModel(8, 6)); // width=8, height=6

        var next = _service.NextState(game);

        Assert.Equal(6, next.Cells.Length);          // rows = height
        Assert.Equal(8, next.Cells[0].Length);       // columns = width
    }
}
