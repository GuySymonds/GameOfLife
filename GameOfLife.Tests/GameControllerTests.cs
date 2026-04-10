using GameOfLife.Api.Controllers;
using GameOfLife.Api.Services;
using GameOfLife.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.Tests;

public class GameControllerTests
{
    // ── AllGames ─────────────────────────────────────────────────────────────

    [Fact]
    public void AllGames_Returns200_WithGameList()
    {
        var service = new StubGameService();
        var controller = new GameController(service);

        var result = controller.AllGames();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsAssignableFrom<IEnumerable<GameModel>>(ok.Value);
    }

    // ── NewGame ───────────────────────────────────────────────────────────────

    [Fact]
    public void NewGame_Returns200_WithCreatedGame()
    {
        var service = new StubGameService();
        var controller = new GameController(service);

        var result = controller.NewGame(new NewGameModel(5, 5));

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<GameModel>(ok.Value);
    }

    // ── GetCurrentState ───────────────────────────────────────────────────────

    [Fact]
    public void GetCurrentState_Returns200_WhenGameExists()
    {
        var id = Guid.NewGuid();
        var service = new StubGameService(games: new Dictionary<Guid, GameModel>
        {
            [id] = new GameModel(id, new bool[][] { [true] })
        });
        var controller = new GameController(service);

        var result = controller.GetCurrentState(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsType<GameModel>(ok.Value);
        Assert.Equal(id, model.GameId);
    }

    [Fact]
    public void GetCurrentState_Returns404_WhenGameNotFound()
    {
        var service = new StubGameService();
        var controller = new GameController(service);

        var result = controller.GetCurrentState(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ── GetNextState ──────────────────────────────────────────────────────────

    [Fact]
    public void GetNextState_Returns400_WhenIdMismatch()
    {
        var service = new StubGameService();
        var controller = new GameController(service);
        var model = new GameModel(Guid.NewGuid(), new bool[][] { [false] });

        var result = controller.GetNextState(Guid.NewGuid(), model);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetNextState_Returns200_WhenIdMatches()
    {
        var id = Guid.NewGuid();
        var service = new StubGameService();
        var controller = new GameController(service);
        var model = new GameModel(id, new bool[][] { [false] });

        var result = controller.GetNextState(id, model);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetNextState_Returns200_WhenBodyGameIdIsEmpty()
    {
        var id = Guid.NewGuid();
        var service = new StubGameService();
        var controller = new GameController(service);
        var model = new GameModel(Guid.Empty, new bool[][] { [false] });

        var result = controller.GetNextState(id, model);

        Assert.IsType<OkObjectResult>(result);
    }

    // ── Stub ──────────────────────────────────────────────────────────────────

    private sealed class StubGameService : IGameService
    {
        private readonly Dictionary<Guid, GameModel> _games;

        public StubGameService(Dictionary<Guid, GameModel>? games = null)
            => _games = games ?? [];

        public IEnumerable<GameModel> AllGames() => _games.Values;

        public GameModel NewGame(NewGameModel model)
        {
            var game = new GameModel(new bool[][] { [false] });
            _games[game.GameId] = game;
            return game;
        }

        public GameModel CurrentState(Guid id)
        {
            if (_games.TryGetValue(id, out var game)) return game;
            throw new KeyNotFoundException($"Key {id} does not exist");
        }

        public GameModel NextState(GameModel model)
        {
            _games[model.GameId] = model;
            return model;
        }
    }
}
