using System;
using System.Linq;
using MSweeper.Core;

namespace MSweeper.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game(8, 8, 8);
            while (!game.GameFinished)
            {
                Console.Write("Which tile should I reveal? ");
                var result = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x) - 1).ToArray();
                game.RevealTile(new TilePosition(result[1], result[0]));
                Console.Clear();
                PrintField(game);
            }
            if (!game.GameWon) {
                Console.WriteLine("Game over!");
            }
            else {
                Console.WriteLine("You won!");
            }
        }

        static void PrintField(Game game)
        {
            var grid = game.GetRevealedGrid();
            for (int i = 0; i < game.Height; i++)
            {
                PrintDivider(game.Width);
                Console.Write(i + 1);
                Console.Write(' ');
                for (int j = 0; j < game.Width; j++)
                {
                    Console.Write("| ");
                    var gridVal = grid[j, i];
                    switch (gridVal)
                    {
                        case -1:
                            Console.Write("#");
                            break;
                        case 0:
                            Console.Write(" ");
                            break;
                        default:
                            Console.ForegroundColor = gridVal switch {
                                1 => ConsoleColor.Blue,
                                2 => ConsoleColor.Green,
                                3 => ConsoleColor.Red,
                                4 => ConsoleColor.Magenta,
                                5 => ConsoleColor.DarkRed,
                                6 => ConsoleColor.DarkGreen,
                                _ => ConsoleColor.White
                            };
                            Console.Write(gridVal);
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                    }
                    Console.Write(' ');
                }
                Console.WriteLine("|");
            }
            PrintDivider(game.Width);
        }
            
        
        static void PrintDivider(int colCount)
        {
            Console.Write("  ");
            for (int i = 0; i < colCount; i++)
            {
                Console.Write("+---");
            }
            Console.WriteLine("+");
        }
    }
}
