using System;
using System.Collections.Generic;
using System.Linq;
using Model.DataTypes;

namespace Model
{
    public class GameModel : IGameModel
    {
        public bool GameEnded { get; private set; }
        public CellPosition Player1Position => _field.Player1Position;
        public CellPosition Player2Position => _field.Player2Position;
        public Field GetField()
        {
            return _field;
        }

        public List<WallPosition> PlacedWalls => _field.PlacedWalls;

        private Field _field;

        //TODO replace with actual implementation
        private IView _view;
        private PlayerNumber _currentPlayer;

        public GameModel()
        {
            _view = null;
            _field = new Field();
        }
        public GameModel(IView view)
        {
            _view = view;
            _field = new Field();
        }

        public void StartNewGame(DrawInView drawInView = DrawInView.Yes)
        {
            GameEnded = false;
            _field = new Field();
            _currentPlayer = PlayerNumber.First;
            if (drawInView == DrawInView.Yes)
                _view.HandleGameStartedEvent();
        }

        public void EndGame(DrawInView drawInView = DrawInView.Yes)
        {
            GameEnded = true;
            if (drawInView == DrawInView.Yes)
               _view.HandleGameEndedEvent();
        }

        public void MovePlayer(PlayerNumber playerNumber, CellPosition newPosition, DrawInView drawInView = DrawInView.Yes)
        {
            if (!IsPlayersTurn(playerNumber))
                throw new AnotherPlayerTurnException($"It is player {_currentPlayer} turn");
            if (!_field.GetCellsForMove(playerNumber).Contains(newPosition))
            {
                throw new IncorrectPlayerPositionException(
                    $"Can't move from {GetPlayerPosition(playerNumber)} to {newPosition}");
            }

            bool isJump = _field.GetCellsForJump(playerNumber).Contains(newPosition);
            _field.MovePlayer(playerNumber, newPosition);
            if (drawInView == DrawInView.Yes)
                _view.HandlePlayerMovedEvent(playerNumber, newPosition, isJump);
            if (IsOnWinningPosition(playerNumber))
            {
                HandleWin(playerNumber, drawInView);
            }

            SwitchCurrentPlayer();
        }

        private bool IsPlayersTurn(PlayerNumber playerNumber)
        {
            return playerNumber == _currentPlayer;
        }

        internal static PlayerNumber GetOppositePlayerNumber(PlayerNumber playerNumber)
        {
            return playerNumber == PlayerNumber.First
                ? PlayerNumber.Second
                : PlayerNumber.First;
        }

        public CellPosition GetPlayerPosition(PlayerNumber playerNumber)
        {
            return playerNumber == PlayerNumber.First
                ? Player1Position
                : Player2Position;
        }

        /// <summary>
        /// For the Quoridor game the situation when opponents face each over face to face is
        /// threatened using some special rules.
        /// </summary>
        /// <returns>Cells available for player to move due face to face situation.
        /// Don't include cells available by regular rules</returns>

        private void SwitchCurrentPlayer()
        {
            _currentPlayer = GetOppositePlayerNumber(_currentPlayer);
        }

        /// <summary>Checks if player is on last(relative to his start position)</summary>
        private bool IsOnWinningPosition(PlayerNumber player)
        {
            CellPosition playerPosition = GetPlayerPosition(player);
            if (player == PlayerNumber.First)
            {
                return playerPosition.Y == 0;
            }
            return playerPosition.Y == GameConstants.FieldEndCoordinate;
        }

        private void HandleWin(PlayerNumber winner, DrawInView drawInView)
        {
            if (drawInView == DrawInView.Yes)
                _view.HandlePlayerWonEvent(winner);
            EndGame();
        }

        public void PlaceWall(PlayerNumber playerPlacing, WallPosition wallPosition, DrawInView drawInView = DrawInView.Yes)
        {
            if (!IsPlayersTurn(playerPlacing))
                throw new AnotherPlayerTurnException($"It is player {_currentPlayer} turn");

            if (!PlayerHasWalls(playerPlacing))
                throw new NoWallsLeftException($"Player {playerPlacing} has no walls left");
            
            _field.PlaceWall(wallPosition, playerPlacing);
            if (drawInView == DrawInView.Yes)
            {
                _view.HandlePlayerPlacedWallEvent(playerPlacing, wallPosition); 
            }
            SwitchCurrentPlayer();
        }

        public List<CellPosition> GetCellsAvailableForMove(PlayerNumber playerNumber)
        {
            return _field.GetCellsForJump(playerNumber).Union(_field.GetCellsForMove(playerNumber)).ToList();
        }

        private bool PlayerHasWalls(PlayerNumber playerNumber)
        {
            int wallAmount = playerNumber == PlayerNumber.First ? _field.Player1WallAmount : _field.Player2WallAmount;
            return wallAmount != 0;
        }
    }
}

