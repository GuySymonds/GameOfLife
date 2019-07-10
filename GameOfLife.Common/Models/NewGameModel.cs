using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife.Common.Models
{
    public class NewGameModel
    {
        public NewGameModel()
        {

        }

        public NewGameModel(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;
    }
}
