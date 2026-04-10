using GameOfLife.Common.Models;
using GameOfLife.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.Api.Controllers
{
    /// <summary>
    /// Manages Conway's Game of Life sessions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        /// <summary>
        /// Initializes a new instance of <see cref="GameController"/>.
        /// </summary>
        /// <param name="gameService">The game service used to manage game state.</param>
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>
        /// Retrieves all active game sessions.
        /// </summary>
        /// <returns>A collection of all <see cref="GameModel"/> instances currently in memory.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GameModel>), 200)]
        public IActionResult AllGames()
        {
            var games = _gameService.AllGames();
            return Ok(games);
        }

        /// <summary>
        /// Creates a new game session with a randomly seeded board.
        /// </summary>
        /// <param name="model">Specifies the <c>Width</c> (columns) and <c>Height</c> (rows) of the board.</param>
        /// <returns>The newly created <see cref="GameModel"/> including its assigned <c>GameId</c> and initial cell state.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(GameModel), 200)]
        [ProducesResponseType(400)]
        public IActionResult NewGame([FromBody] NewGameModel model)
        {
            var game = _gameService.NewGame(model);
            return Ok(game);
        }

        /// <summary>
        /// Advances the supplied game state by one generation and returns the result.
        /// The full <see cref="GameModel"/> must be included in the request body so that
        /// the server can compute the next generation from the cells provided by the client,
        /// making the endpoint stateless with respect to the board data.
        /// </summary>
        /// <param name="id">The unique identifier of the game session.</param>
        /// <param name="model">The current game state, including the <c>Cells</c> grid to advance.</param>
        /// <returns>A <see cref="GameModel"/> representing the next generation.</returns>
        [HttpPost("{id}/next")]
        [ProducesResponseType(typeof(GameModel), 200)]
        [ProducesResponseType(400)]
        public IActionResult GetNextState(Guid id, [FromBody] GameModel model)
        {
            if (model.GameId != Guid.Empty && model.GameId != id)
                return BadRequest($"Route id '{id}' does not match body GameId '{model.GameId}'.");

            model.GameId = id;
            var game = _gameService.NextState(model);
            return Ok(game);
        }

        /// <summary>
        /// Retrieves the current state of an existing game session.
        /// </summary>
        /// <param name="id">The unique identifier of the game session.</param>
        /// <returns>The <see cref="GameModel"/> for the requested game, including its current <c>Cells</c> grid.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GameModel), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetCurrentState(Guid id)
        {
            try
            {
                var game = _gameService.CurrentState(id);
                return Ok(game);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"No game found with id '{id}'.");
            }
        }
    }
}
