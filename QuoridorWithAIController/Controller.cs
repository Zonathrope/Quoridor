using System;
using System.Collections.Generic;
using Model;
using Model.DataTypes;

namespace QuoridorWithAIController
{
    //TODO replace mocks
    internal class AIMock : IAI
    {
        public Object GetMove()
        {
            throw new NotImplementedException();
        }
    }
    
    internal class ViewMock : IView
    {
        public void HandleGameStartedEvent()
        {
            throw new NotImplementedException();
        }

        public void HandleGameEndedEvent()
        {
            throw new NotImplementedException();
        }

        public void HandlePlayerWonEvent(PlayerNumber winnerNumber)
        {
            throw new NotImplementedException();
        }

        public void HandlePlayerMovedEvent(PlayerNumber playerNumber, CellPosition newPosition, bool isJump = false)
        {
            throw new NotImplementedException();
        }

        public void HandlePlayerPlacedWallEvent(PlayerNumber playerPlacing, WallPosition wallPosition)
        {
            throw new NotImplementedException();
        }
    }

    internal class Controller
    {
        //TODO make more flexible
        private IAI _ai = new AIMock();
        private IView _view = new ViewMock();
        private IGameModel _gameModel;
        private PlayerNumber _AINumber;

        private readonly Dictionary<Char, int> letterToXCordinate = new()
        {
            {'A', 0}, {'B', 1}, {'C', 2}, {'D', 3}, {'E', 4}, {'F', 5}, {'G', 6}, {'H', 7}, {'I', 8},
            {'S', 0}, {'T', 1}, {'U', 2}, {'V', 3}, {'W', 4}, {'X', 5}, {'Y', 6}, {'Z', 7}
        };

        public Controller()
        {
            _gameModel = new GameModel(_view);
        }

        public void Start()
        {
            string startInput = Console.ReadLine();
            _AINumber = startInput == "black" ? PlayerNumber.First : PlayerNumber.Second;
            if (_AINumber == PlayerNumber.Second)
            {
                string opponentInput = Console.ReadLine();
                HandleOpponentMove(opponentInput);
            }

            while (!_gameModel.GameEnded)
            {
                Object move = _ai.GetMove();
                HandleAIMove(move);
                string opponentInput = Console.ReadLine();
                HandleOpponentMove(opponentInput);
            }
        }

        private void HandleAIMove(object move)
        {
            throw new NotImplementedException();
        }

        private void HandleOpponentMove(string move)
        {
            string[] moveParts = move.Split(' ');
            string command = moveParts[0];
            string location = moveParts[1];
            PlayerNumber opponentNumber = GetOppositePlayer(_AINumber);
            if (command is "move" or "jump")
                HandleOpponentMovePlayer(opponentNumber, location);
            else
                HandleOpponentPlaceWall(opponentNumber, location);
        }

        private void HandleOpponentMovePlayer(PlayerNumber opponentNumber, string location)
        {
            CellPosition newPosition = ParseCellPosition(location);
            _gameModel.MovePlayer(opponentNumber, newPosition, DrawInView.No);
        }

        private CellPosition ParseCellPosition(string location)
        {
            char letter = location[0];
            string number = location[1].ToString();
            int x = letterToXCordinate[letter];
            // -1 to shift to zero indexing
            int y = Int32.Parse(number) - 1;
            return new CellPosition(x, y);
        }

        private void HandleOpponentPlaceWall(PlayerNumber opponentNumber, string location)
        {
            WallPosition wallPosition = ParseWallPosition(location);
            _gameModel.PlaceWall(opponentNumber, wallPosition);
        }

        private WallPosition ParseWallPosition(string location)
        {
            string cellPositionString = location.Substring(0, 2);
            CellPosition cellPosition = ParseCellPosition(cellPositionString);
            char wallOrientationChar = location[2];
            WallOrientation wallOrientation =
                wallOrientationChar == 'h'
                    ? WallOrientation.Horizontal
                    : WallOrientation.Vertical;
            return new WallPosition(wallOrientation, cellPosition);
        }
        private PlayerNumber GetOppositePlayer(PlayerNumber playerNumber)
        {
            return playerNumber == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;
        }
    }
}