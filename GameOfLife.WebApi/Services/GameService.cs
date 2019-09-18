using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.Common.Models;

namespace GameOfLife.Engine.Services
{
    /// <summary>
    /// ------- The Rules ------- 
    /// --- For a space that is 'populated':
    /// Each cell with one or no neighbors dies, as if by solitude.
    /// Each cell with four or more neighbors dies, as if by overpopulation. 
    /// Each cell with two or three neighbors survives. 
    /// 
    /// --- For a space that is 'empty' or 'unpopulated':
    /// Each cell with three neighbors becomes populated.
    /// </summary>
    public class GameService : IGameService
    {
        private readonly GameEngine _gameEngine;

        public GameService(GameEngine gameEngine)
        {
            _gameEngine = gameEngine;
            if (!DataStore.ContainsKey(Guid.Parse("e6829659-e497-461d-8313-2993b9a3d9e8")))
                DataStore.Add(
                    Guid.Parse("e6829659-e497-461d-8313-2993b9a3d9e8"),
                    new GameModel
                    {
                        GameId = Guid.Parse("e6829659-e497-461d-8313-2993b9a3d9e8"),
                        Cells = _gameEngine.GenerateSeed(18, 40)
                    });
        }

        private static readonly Lazy<IDictionary<Guid, GameModel>> _dataStore =
            new Lazy<IDictionary<Guid, GameModel>>(() =>
                new Dictionary<Guid, GameModel>());

        public IDictionary<Guid, GameModel> DataStore => _dataStore.Value;

        public IEnumerable<GameModel> AllGames()
        {
            return DataStore.Select(x => x.Value);
        }

        public GameModel CurrentState(Guid id)
        {
            if (DataStore.ContainsKey(id))
                return DataStore[id];
            throw new KeyNotFoundException($"Key {id} does not exist");
        }

        public GameModel NewGame(NewGameModel model)
        {
            var game = new GameModel(_gameEngine.GenerateSeed(model.Width, model.Height));
            DataStore.Add(game.GameId, game);
            return game;
        }

        public GameModel NextState(Guid id)
        {
            if (!DataStore.ContainsKey(id))
                throw new KeyNotFoundException($"Key {id} does not exist");

            DataStore[id].Cells = _gameEngine.GetNextState(DataStore[id].Cells);
            return DataStore[id];
        }
    }
}
