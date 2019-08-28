using System;
using System.Collections.Generic;

namespace MSweeper.Core
{
    public class Game
    {
        private HashSet<TilePosition> _mines;
        private int[,] _revealed;
        private int _width;
        private int _height;
        private int _mineCount;
        private int _remainingTiles;

        public int[,] GetRevealedGrid() => _revealed.Clone() as int[,];

        public int Width => _width;
        public int Height => _height;
        public int MineCount => _mineCount;

        public bool GameFinished { get; private set; } = false;
        public bool GameWon { get; private set; } = false;

        public Game(int width, int height, int mineCount) 
        {
            _width = width;
            _height = height;
            _mineCount = mineCount;
            _mines = new HashSet<TilePosition>();
            _revealed = new int[_width, _height];
            _remainingTiles = _width * _height - _mineCount;
            for (int i = 0; i < _width; i++)
            for (int j = 0; j < _height; j++)
            {
                _revealed[i, j] = -1;
            }
            CreateMines();
        }

        public bool RevealTile(TilePosition tile)
        {
            if (_revealed[tile.X, tile.Y] != -1) return false;
            GameFinished = _mines.Contains(tile);
            if (GameFinished) return true;
            var initialCount = AdjacentCount(tile);
            _revealed[tile.X, tile.Y] = initialCount;
            _remainingTiles--;
            GameFinished = _remainingTiles == 0;
            if (GameFinished) GameWon = true;
            if (initialCount != 0) return false;
            for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                var newX = tile.X + x;
                var newY = tile.Y + y;
                if (newX >= 0 && newX < _width &&
                    newY >= 0 && newY < _height) {
                    if (_revealed[newX, newY] != -1) continue;
                    RevealTile(new TilePosition(newX, newY));
                }
            }
            return false;
        }

        private byte AdjacentCount(TilePosition tile)
        {
            byte count = 0;
            for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                if (_mines.Contains(new TilePosition(tile.X + x, tile.Y +y)))
                {
                    count++;
                }
            }
            return count;
        }

        private void CreateMines()
        {
            var random = new Random((int)System.DateTime.Now.Ticks);
            var toAdd = _mineCount;
            while (toAdd > 0)
            {
                var tile = new TilePosition(random.Next(_width), random.Next(_height));
                if (_mines.Contains(tile)) continue;
                _mines.Add(tile);
                toAdd--;
            }
        }
    }
}
