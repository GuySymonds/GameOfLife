using GameOfLife.Common.Models;
using GameOfLife.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        // GET api/games
        [HttpGet]
        public IActionResult AllGames()
        {
            var games = _gameService.AllGames();
            return Ok(games);
        }

        // POST api/games
        [HttpPost]
        public IActionResult NewGame([FromBody] NewGameModel model)
        {
            var game = _gameService.NewGame(model);
            return Ok(game);
        }

        // GET api/games/next/e6829659-e497-461d-8313-2993b9a3d9e8
        [HttpGet("{id}/next")]
        public IActionResult GetNextState(Guid id)
        {
            var game = _gameService.NextState(id);
            return Ok(game);
        }

        // GET api/games/e6829659-e497-461d-8313-2993b9a3d9e8
        [HttpGet("{id}")]
        public IActionResult GetCurrentState(Guid id)
        {
            var game = _gameService.CurrentState(id);
            return Ok(game);
        }

    }
}
