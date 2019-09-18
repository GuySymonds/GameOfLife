using System;
using System.Threading.Tasks;
using GameOfLife.Common.Models;

namespace GameOfLife.ConApp
{
    public interface IGame
    {
        GameModel GetGameState(Guid id);
        GameModel GetNewGame(NewGameModel model);
        GameModel GetNextGameState(Guid id);
        Task<GameModel> GetGameStateAsync(Guid id);
        Task<GameModel> GetNewGameAsync(NewGameModel model);
        Task<GameModel> GetNextGameStateAsync(Guid id);
    }
}