using System;
using System.Collections.Generic;
using System.Linq;

namespace MSweeper.Core
{
    public class Game
    {
        private readonly HashSet<TilePosition> _mines;
        private readonly GameTile[,] _revealed;
        private readonly int _width;
        private readonly int _height;
        private readonly int _mineCount;
        private int _remainingTiles;

        public GameTile[,] GetRevealedGrid() => _revealed; //_revealed.Clone() as GameTile[,];
        public HashSet<TilePosition> GetMines() => GameFinished ? _mines : throw new InvalidOperationException();

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
            _revealed = new GameTile[_width, _height];
            _remainingTiles = _width * _height - _mineCount;
            for (int i = 0; i < _width; i++)
            for (int j = 0; j < _height; j++)
            {
                _revealed[i, j] = new GameTile { Position = new TilePosition(i ,j) };
            }
            for (int i = 0; i < _width; i++)
            for (int j = 0; j < _height; j++)
            {
                var centerTile = _revealed[i, j];
                for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    var newX = i + x;
                    var newY = j + y;
                    if (newX >= 0 && newX < _width &&
                        newY >= 0 && newY < _height) {
                        centerTile.Neighbors.Add(_revealed[newX, newY]);
                    }
                }
            }
            CreateMines();
        }

        public void Reset()
        {
            GameFinished = false;
            GameWon = false;
            _remainingTiles = _width * _height - _mineCount;
            for (int i = 0; i < _width; i++)
            for (int j = 0; j < _height; j++)
            {
                var tile = _revealed[i, j];
                tile.IsMine = false;
                tile.Value = -1;
            }
            _mines.Clear();
            CreateMines();
        }

        public RevealResult RevealTile(TilePosition tile)
        {
            var tileObj = _revealed[tile.X, tile.Y];
            // we've already revealed this tile
            if (tileObj.Value != -1)
                return RevealResult.NoChange;
            // check if the player selected a mine
            GameFinished = _mines.Contains(tile);
            // if so, the game is over and they lost
            if (GameFinished) return RevealResult.GameLost;
            // otherwise, reveal this tile
            var initialCount = AdjacentCount(tileObj);
            tileObj.Value = initialCount;
            // we now need one fewer tiles to be revealed to win the game
            _remainingTiles--;
            GameFinished = _remainingTiles == 0;
            if (GameFinished)
            {
                GameWon = true;
                return RevealResult.GameWon;
            }
            if (initialCount != 0) return RevealResult.Ok;
            foreach (var adjTile in tileObj.Neighbors) {
                if (adjTile.Value == -1) RevealTile(adjTile.Position);
            }
            return RevealResult.Ok;
        }

        private int AdjacentCount(GameTile tile)
        {
            return tile.Neighbors.Where(x => x.IsMine).Count();
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
                _revealed[tile.X, tile.Y].IsMine = true;
                toAdd--;
            }
        }
    }
}
