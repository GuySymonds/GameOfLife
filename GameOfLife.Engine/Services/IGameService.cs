using GameOfLife.Common.Models;
using System;
using System.Collections.Generic;

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
    public interface IGameService
    {
        GameModel NewGame(NewGameModel model);
        GameModel NextState(Guid id);
        IEnumerable<GameModel> AllGames();
        GameModel CurrentState(Guid id);
    }
}
