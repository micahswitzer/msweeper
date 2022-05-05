using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MSweeper.Core;

namespace MSweeper.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome! Computing results...");
            int width = int.Parse(args[0]);
            int height = int.Parse(args[1]);
            int mineCount = int.Parse(args[2]);
            int totalNum = int.Parse(args[3]);
            int won = 0, lost = 0;
            int numCompleted = totalNum;
            var tasks = new List<Task>(8);
            for (int i = 0; i < 8; i++)
            {
                var threadNum = i;
                tasks.Add(Task.Run(() => {
                    var game = new Game(width, height, mineCount);
                    var firstRun = true;
                    while (numCompleted > 0) {
                        var runNum = numCompleted--;
                        //Console.WriteLine($"Starting run #{totalNum - runNum} on thread {threadNum}");
                        if (!firstRun) {
                            game.Reset();
                        }
                        firstRun = false;
                        var ai = new MSweeper.AI.FastAI();
                        while (!game.GameFinished)
                        {
                            //Console.Write("Which tile should I reveal? ");
                            //var result = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x) - 1).ToArray();
                            //game.RevealTile(new TilePosition(result[1], result[0]));
                            //Console.ReadLine();
                            //Thread.Sleep(100);
                            //Console.Clear();
                            game.RevealTile(ai.ChooseTile(game));
                            //PrintField(game);
                        }
                        if (!game.GameWon) {
                            lost++;
                            Console.Write("You lost!");
                        }
                        else {
                            won++;
                            Console.Write("You won!");
                        }
                        //PrintField(game);
                        Console.WriteLine($" Guess percentage: {string.Format("{0:F2}", ai.GuessesMade * 1.0 / ai.ChoicesMade * 100.0)}%");
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Games won: {won}, Games lost: {lost}, Win percentage: {won * 1.0 / (won + lost) * 100}%");
        }

        static void PrintField(Game game)
        {
            var grid = game.GetRevealedGrid();
            var mines = game.GameFinished ? game.GetMines() : null;
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
                    switch (gridVal.Value)
                    {
                        case -1:
                            var isMine = (mines?.Contains(gridVal.Position) ?? false);
                            if (isMine) {
                                var origFgColor = Console.ForegroundColor;
                                var origBgColor = Console.BackgroundColor;
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.Write('*');
                                Console.ForegroundColor = origFgColor;
                                Console.BackgroundColor = origBgColor;
                            }
                            else {
                                Console.Write("#");
                            }
                            break;
                        case 0:
                            Console.Write(" ");
                            break;
                        default:
                            var preColor = Console.ForegroundColor;
                            Console.ForegroundColor = gridVal.Value switch {
                                1 => ConsoleColor.Blue,
                                2 => ConsoleColor.Green,
                                3 => ConsoleColor.Red,
                                4 => ConsoleColor.Magenta,
                                5 => ConsoleColor.DarkRed,
                                6 => ConsoleColor.DarkGreen,
                                7 => ConsoleColor.DarkMagenta,
                                8 => ConsoleColor.DarkYellow,
                                _ => preColor
                            };
                            Console.Write(gridVal.Value);
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
