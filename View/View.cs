using System;
using System.Collections.Generic;
using Model;
using Model.DataTypes;

namespace View
{
    public class View : IView
    {
        private readonly Dictionary<int, char> _xToLetterForMoves = new()
        {{0, 'A'}, {1, 'B'}, {2, 'C'}, {3, 'D'}, {4, 'E'}, {5, 'F'}, {6, 'G'}, {7, 'H'}, {8, 'I'}};
        private readonly Dictionary<int, char> _xToLetterForWalls = new()
            {{0, 'S'}, {1, 'T'}, {2, 'U'}, {3, 'V'}, {4, 'W'}, {5, 'X'}, {6, 'Y'}, {7, 'Z'}};

        public void HandleGameStartedEvent() { }
        public void HandleGameEndedEvent() { }
        public void HandlePlayerWonEvent(PlayerNumber winnerNumber) { }

        public void HandlePlayerMovedEvent(PlayerNumber playerNumber, CellPosition newPosition, bool isJump = false)
        {
            string command = isJump ? "jump" : "move";
            string coordinate = MovePositionToString(newPosition);
            Console.WriteLine($"{command} {coordinate}");
        }

        public void HandlePlayerPlacedWallEvent(PlayerNumber playerPlacing, WallPosition wallPosition)
        {
            string stringWallPosition = WallPositionToString(wallPosition);
            Console.WriteLine($"wall {stringWallPosition}");
        }

        private string MovePositionToString(CellPosition cellPosition)
        {
            string x = _xToLetterForMoves[cellPosition.X].ToString();
            string y = (cellPosition.Y + 1).ToString();
            return x + y;
        }

        private string WallPositionToString(WallPosition wallPosition)
        {
            var cellPosition = wallPosition.TopLeftCell;
            string x = _xToLetterForWalls[cellPosition.X].ToString();
            string y = (cellPosition.Y + 1).ToString();
            string coordinates = x + y;
            string orientation = wallPosition.Orientation == WallOrientation.Horizontal ? "h" : "v";
            return coordinates + orientation;
        }
    }
}