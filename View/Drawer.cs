using System;
using Quoridor.View.Constants;
using Quoridor.View.Interfaces;

namespace Quoridor.View
{
    public class Drawer
    {
        public void ShowStartInfo()
        {
            Console.WriteLine(" WELCOME TO\r\nTHE QUORIDOR\r\n");
        }

        public void ShowGameModeOptions()
        {
            Console.WriteLine("\r\nChoose a game mode firstly\r\n 1 - Against PC\r\n 2 - Local multiplayer");
        }

        public void DrawBoard()
        {
            for (var i = 0; i < 17; i++)
            {
                for (var j = 0; j < 18; j++)
                {
                    if (i % 2 == 0 && j % 2 == 0)
                    {
                        Console.Write(DrawConstants.EmptyCell);
                    }

                    if ((j % 2 != 0) && (i % 2 != 0))
                    {
                        Console.Write(DrawConstants.TopDownWall);
                    }

                    if (j != 17 && j % 2 != 0)
                    {
                        Console.Write(DrawConstants.LeftRightWall);
                    }
                }

                Console.Write("\n");
            }
        }

        public void WriteMoves(string[] possibleMoves)
        {
            for(var i = 1; i < possibleMoves.Length; i++)
            {
                Console.WriteLine(i + " - " + possibleMoves[i]);
            }
        }

        public void EndGame()
        {
            throw new NotImplementedException();
        }

        public void ClearConsole()
        {
            Console.Clear();
        }
    }
}