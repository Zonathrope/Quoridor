using System;
using System.Collections.Generic;
using Model.DataTypes;
using Model.Internal;

namespace OldController
{
    public class Drawer
    {
        public void ShowStartInfo()
        {
            Console.WriteLine(" WELCOME TO\r\nTHE QUORIDOR\r\n");
        }

        public void ChooseGameMode()
        {
            Console.WriteLine("\r\nChoose a game mode firstly\r\n 1 - Against PC\r\n 2 - Local multiplayer");
        }

        public void DrawTurnOptions(Boolean hasWalls)
        {
            if(hasWalls)
                Console.WriteLine(" 1 - Move \n 2 - Place wall");
            else
                Console.WriteLine("1 - Move");
        }

        public void DrawMoveOptions(List<CellPosition> possibleMoves)
        {
            var counter = 1;
            foreach (var possibleMove in possibleMoves)
            {
                Console.WriteLine(counter + " " + possibleMove);
                counter++;
            }
        }

        public void DrawBoard(string[,] board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    Console.Write(board[i,j]);
                }
                Console.WriteLine();
            }
        }

        public void DrawWallOption()
        {
            Console.WriteLine("Write top-left cell and orientation: {X Y H/V}");
        }

        public void DrawWinner(PlayerNumber player)
        {
            Console.Clear();
            Console.WriteLine(player == PlayerNumber.First ? "Player 2 WIN!" : "Player 1 WIN!");
        }

        public void ClearConsole()
        {
            Console.Clear();
        }
    }
}