using System;
using System.Collections.Generic;
using System.Linq;
using MSweeper.Core;

namespace MSweeper.AI
{
    public class FastAI : IGamePlayer
    {
        private ISet<TilePosition> _flags;
        private readonly Random _random = new Random((int)DateTime.Now.Ticks);

        public FastAI()
        {
            _flags = new HashSet<TilePosition>();
        }

        public TilePosition ChooseTile(Game game)
        {
            var grid = game.GetRevealedGrid();
            for (int i = 0; i < game.Width; i++)
            for (int j = 0; j < game.Height; j++)
            {
                var tileVal = grid[i, j];
                if (tileVal < 1) continue;
                var unrevealed = Unrevealed(game, grid, new TilePosition(i, j));
                if (unrevealed.Count == tileVal)
                    foreach (var tile in unrevealed)
                        _flags.Add(tile);
            }
            for (int i = 0; i < game.Width; i++)
            for (int j = 0; j < game.Height; j++)
            {
                var tileVal = grid[i, j];
                if (tileVal < 1) continue;
                var unrevealed = Unrevealed(game, grid, new TilePosition(i, j));
                var unrevealedCount = unrevealed.Count;
                unrevealed.ExceptWith(_flags);
                unrevealedCount -= unrevealed.Count;
                if (unrevealedCount == tileVal && unrevealed.Any()) {
                    Console.WriteLine("I know this isn't a mine.");
                    return unrevealed.First();
                }
            }
            Console.WriteLine("Guessing, wish me luck!");
            return RandomGuess(game, grid);
        }

        private ISet<TilePosition> Unrevealed(Game game, int[,] grid, TilePosition tile)
        {
            var list = new HashSet<TilePosition>();
            for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
                var x = tile.X + i;
                var y = tile.Y + j;
                if (x >= game.Width || x < 0 || y >= game.Height || y < 0) continue;
                if (grid[x, y] == -1) list.Add(new TilePosition(x, y));
            }
            return list;
        }

        private ISet<TilePosition> Flags(Game game, int[,] grid, TilePosition tile)
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

        private TilePosition RandomGuess(Game game, int[,] grid)
        {
            while (true)
            {
                var x = _random.Next(game.Width);
                var y = _random.Next(game.Height);
                var tile = new TilePosition(x, y);
                if (_flags.Contains(tile) || grid[x, y] != -1) continue;
                return tile;
            }
        }
    }
}
