using System;
using System.Collections.Generic;
using System.Linq;
using MSweeper.Core;

namespace MSweeper.AI
{
    public class FastAI : IGamePlayer
    {
        private ISet<TilePosition> _flags;
        private ISet<TilePosition> _toUncover;
        private readonly Random _random = new Random((int)DateTime.Now.Ticks);

        public int ChoicesMade { get; private set; }
        public int GuessesMade { get; private set; }

        public FastAI()
        {
            _flags = new HashSet<TilePosition>();
            _toUncover = new HashSet<TilePosition>();
        }

        public TilePosition ChooseTile(Game game)
        {
            ChoicesMade++;
            if (_toUncover.Count > 0) {
                var tile = _toUncover.First();
                _toUncover.Remove(tile);
                return tile;
            }
            var grid = game.GetRevealedGrid();
            for (int i = 0; i < game.Width; i++)
            for (int j = 0; j < game.Height; j++)
            {
                var tileVal = grid[i, j].Value;
                if (tileVal < 1) continue;
                var unrevealed = Unrevealed(game, grid, new TilePosition(i, j));
                if (unrevealed.Count == tileVal)
                    foreach (var tile in unrevealed)
                        _flags.Add(tile);
            }
            for (int i = 0; i < game.Width; i++)
            for (int j = 0; j < game.Height; j++)
            {
                var tileVal = grid[i, j].Value;
                if (tileVal < 1) continue;
                var unrevealed = Unrevealed(game, grid, new TilePosition(i, j));
                var unrevealedCount = unrevealed.Count;
                unrevealed.ExceptWith(_flags);
                unrevealedCount -= unrevealed.Count;
                if (unrevealedCount == tileVal && unrevealed.Any()) {
                    //Console.WriteLine("I know this isn't a mine.");
                    foreach (var tile in unrevealed.Skip(1)) _toUncover.Add(tile);
                    return unrevealed.First();
                }
            }
            //Console.WriteLine("Guessing, wish me luck!");
            GuessesMade++;
            return RandomGuess(game, grid);
        }

        private ISet<TilePosition> Unrevealed(Game game, GameTile[,] grid, TilePosition tile)
        {
            var list = new HashSet<TilePosition>();
            for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
                var x = tile.X + i;
                var y = tile.Y + j;
                if (x >= game.Width || x < 0 || y >= game.Height || y < 0) continue;
                var gameTile = grid[x, y];
                if (gameTile.Value == -1) list.Add(gameTile.Position);
            }
            return list;
        }

        private ISet<TilePosition> Flags(Game game, GameTile[,] grid, TilePosition tile)
        {
            var list = new HashSet<TilePosition>();
            for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
                var x = tile.X + i;
                var y = tile.Y + j;
                if (x >= game.Width || x < 0 || y >= game.Height || y < 0) continue;
                var newTile = new TilePosition(x, y);
                if (_flags.Contains(newTile)) list.Add(newTile);
            }
            return list;
        }

        private TilePosition RandomGuess(Game game, GameTile[,] grid)
        {
            while (true)
            {
                var x = _random.Next(game.Width);
                var y = _random.Next(game.Height);
                var tile = new TilePosition(x, y);
                if (_flags.Contains(tile) || grid[x, y].Value != -1) continue;
                return tile;
            }
        }
    }
}
