using System;
using System.Linq;
using MSweeper.Core;

namespace MSweeper.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game(8, 12, 15);
            var ai = new MSweeper.AI.FastAI();
            Console.WriteLine("Welcome! Press Enter to begin.");
            while (!game.GameFinished)
            {
                //Console.Write("Which tile should I reveal? ");
                //var result = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x) - 1).ToArray();
                //game.RevealTile(new TilePosition(result[1], result[0]));
                Console.ReadLine();
                Console.Clear();
                game.RevealTile(ai.ChooseTile(game));
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
                PrintDivider(game);
                var numString = (i + 1).ToString();
                var numSpaces = Math.Floor(Math.Log10(game.Height)) - numString.Length + 2;
                Console.Write(numString);
                for (int j = 0; j < numSpaces; j++)
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
                            var preColor = Console.ForegroundColor;
                            Console.ForegroundColor = gridVal switch {
                                1 => ConsoleColor.Blue,
                                2 => ConsoleColor.Green,
                                3 => ConsoleColor.Red,
                                4 => ConsoleColor.Magenta,
                                5 => ConsoleColor.DarkRed,
                                6 => ConsoleColor.DarkGreen,
                                _ => preColor
                            };
                            Console.Write(gridVal);
                            Console.ForegroundColor = preColor;
                            break;
                    }
                    Console.Write(' ');
                }
                Console.WriteLine("|");
            }
            PrintDivider(game);
        }
        
        static void PrintDivider(Game game)
        {
            for (int i = 0; i < Math.Floor(Math.Log10(game.Height)) + 2; i++)
            {
                Console.Write(' ');
            }
            for (int i = 0; i < game.Width; i++)
            {
                Console.Write("+---");
            }
            Console.WriteLine("+");
        }
    }
}
