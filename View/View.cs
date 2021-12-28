using System;
using System.Collections.Generic;
using Model;
using Model.DataTypes;

namespace View
{
    public class View : IView
    {
        private readonly Dictionary<int, char> _xToLetterForMoves = new()
        {{0, 'a'}, {1, 'b'}, {2, 'c'}, {3, 'd'}, {4, 'e'}, {5, 'f'}, {6, 'g'}, {7, 'h'}, {8, 'i'}};
        private readonly Dictionary<int, char> _xToLetterForWalls = new()
            {{0, 's'}, {1, 't'}, {2, 'u'}, {3, 'v'}, {4, 'w'}, {5, 'x'}, {6, 'y'}, {7, 'z'}};

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