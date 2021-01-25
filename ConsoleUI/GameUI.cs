using System;
using System.ComponentModel;
using GameEngine;

namespace ConsoleUI
{
    public static class GameUi
    {
        private static readonly string _verticalSeparator = "|";
        private static readonly string _horizontalSeparator = "-";
        private static readonly string _centerSeparator = "+";
        
        public static void PrintBoard(Game game)
        {
            string line;
            var board = game.GetBoard();
            line = "";
            for (var xIndex = 0; xIndex <= game.BoardWidth; xIndex++)
            {
                line += _centerSeparator;
                if (xIndex < game.BoardWidth)
                {
                    line = line + _horizontalSeparator + _horizontalSeparator + _horizontalSeparator;
                }
            }
            Console.WriteLine(line);
            for (var yIndex = 0; yIndex <= game.BoardHeight; yIndex++)
            {
                if (yIndex >= game.BoardHeight) continue;
                line = "";
                for (var xIndex = 0; xIndex <= game.BoardWidth; xIndex++)
                {
                    line += _verticalSeparator;
                    if (xIndex < game.BoardWidth)
                    {
                        line = line + " " + GetSingleState(board[yIndex, xIndex]) + " ";
                    }
                }
                Console.WriteLine(line);
                
                if (yIndex > game.BoardHeight + 1) continue;
                line = "";
                for (var xIndex = 0; xIndex <= game.BoardWidth; xIndex++)
                {
                    line += _centerSeparator;
                    if (xIndex < game.BoardWidth)
                    {
                        line = line + _horizontalSeparator + _horizontalSeparator + _horizontalSeparator;
                    }
                }
                Console.WriteLine(line);
            }

            line = "";
            for (int i = 1; i <= game.BoardWidth; i++)
            {
                line += "  " + i + " ";
            }
            Console.WriteLine(line);
        }

        public static void PrintVictoryScreen()
        {
            var line = "";
            line += "========================\n"
                    + $"      GAME OVER! \n    THE WINNER IS: \n       {(Game.Draw ? "EVERYONE" : Game.PlayerXMove ? "   O" : "   X")}\n"
                    + "========================";
            Console.WriteLine(line);
        }

        public static string GetSingleState(CellState state)
        {
            switch (state)
            {
                case CellState.Empty:
                    return " ";
                case CellState.O:
                    return "O";
                case CellState.X:
                    return "X";
                default:
                    throw new InvalidEnumArgumentException("Unknown enum option!");
            }
            
        }
    }
}