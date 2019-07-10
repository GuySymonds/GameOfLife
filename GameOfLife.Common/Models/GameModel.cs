using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife.Common.Models
{
    public class GameModel
    {
        public GameModel() { }

        public GameModel(byte[,] data) : this(Guid.NewGuid(), data) { }

        public GameModel(Guid id, byte[,] data)
        {
            GameId = id;
            Cells = data;
        }

        public Guid GameId { get; set; }
        public byte[,] Cells { get; set; }
    }
}
