using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Models;
using GameOfLife;

namespace Engine.Services
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
        private static readonly Lazy<IDictionary<Guid, GameModel>> _dataStore = 
            new Lazy<IDictionary<Guid, GameModel>>(()=> new Dictionary<Guid, GameModel>());

        private static readonly Lazy<Random> _rand = new Lazy<Random>(()=>new Random());

        public Random Rand => _rand.Value;
        public IDictionary<Guid, GameModel> DataStore => _dataStore.Value;

        public IEnumerable<GameModel> AllGames()
        {
            return DataStore.Select(x=>x.Value);
        }

        public GameModel CurrentState(Guid id)
        {
            if (DataStore.ContainsKey(id))
                return DataStore[id];
            throw new KeyNotFoundException($"Key {id} does not exist");
        }

        public GameModel NewGame(NewGameModel model)
        {
            var game = new GameModel(GenerateSeed(model.Width, model.Height));
            DataStore.Add(game.GameId, game);
            return game;
        }

        public GameModel NextState(Guid id)
        {
            if(DataStore.ContainsKey(id))
            {
                DataStore[id].Cells = Tick(DataStore[id].Cells);
                return DataStore[id];
            }
            throw new KeyNotFoundException($"Key {id} does not exist");
        }

        private bool[,] GenerateSeed(int rows, int columns)
        {
            bool[,] seed = new bool[rows, columns];
            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    seed[row, column] = _rand.Value.NextDouble() < 0.2 ? true : false;
                }
            }

            return seed;
        }

        public bool[,] Tick(bool[,] game)
        {
            var rows = game.GetLength(0);
            var columns = game.GetLength(1);

            var future = new bool[rows, columns];

            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    future[row, column] = game.Law(row, column);
                }
            }

            return future;
        }
    }
}
