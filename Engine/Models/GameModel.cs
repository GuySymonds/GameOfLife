using System;

namespace Engine.Models
{
    public class GameModel
    {
        public GameModel() { }

        public GameModel(bool[,] data) : this(Guid.NewGuid(), data) { }

        public GameModel(Guid id, bool[,] data)
        {
            GameId = id;
            Cells = data;
        }

        public Guid GameId { get; set; }
        public bool[,] Cells { get; set; }
    }
}
