using System.Collections.Generic;
using System.Linq;
using Model.DataTypes;
using Model.Internal;

namespace Model
{
    public class GameModel : IGameModel
    {
        public CellPosition Player1Position => _field.Player1Position;
        public CellPosition Player2Position => _field.Player2Position;
        public int Player1WallAmount { get; private set; }
        public int Player2WallAmount { get; private set; }
        public List<WallPosition> PlacedWalls => _field.PlacedWalls;

        private Field _field;

        public Field GetField()
        {
            return _field;
        }
        
        //TODO replace with actual implementation
        private IAStar _aStar = new AStar();
        private PlayerNumber _currentPlayer;


        public GameModel()
        {
            //AttachEventsToPlayer(player1);
            //AttachEventsToPlayer(player2);
            _field = new Field();
            Player1WallAmount = GameConstants.StartWallAmount;
            Player2WallAmount = GameConstants.StartWallAmount;
        }

        public void StartNewGame()
        {
            _field = new Field();
            _currentPlayer = PlayerNumber.First;
            Player1WallAmount = GameConstants.StartWallAmount;
            Player2WallAmount = GameConstants.StartWallAmount;
        }

        public void EndGame()
        {
        }

        public void MovePlayer(PlayerNumber playerNumber, CellPosition newPosition)
        {
            if (!IsPlayersTurn(playerNumber))
                throw new AnotherPlayerTurnException($"It is player {_currentPlayer} turn");

            if (!GetCellsAvailableForMove(playerNumber).Contains(newPosition))
            {
                throw new IncorrectPlayerPositionException(
                    $"Can't move from {GetPlayerPosition(playerNumber)} to {newPosition}");
            }

            _field.MovePlayer(playerNumber, newPosition);
            if (IsOnWinningPosition(playerNumber))
            {
                HandleWin(playerNumber);
            }

            SwitchCurrentPlayer();
        }

        private bool IsPlayersTurn(PlayerNumber playerNumber)
        {
            return playerNumber == _currentPlayer;
        }

        public List<CellPosition> GetCellsAvailableForMove(PlayerNumber playerNumber)
        {
            CellPosition playerCurrentPosition = GetPlayerPosition(playerNumber);
            CellPosition opponentPosition = GetPlayerPosition(GetOppositePlayerNumber(playerNumber));
            List<CellPosition> reachableCells = _field.GetReachableNeighbours(playerCurrentPosition);
            if (reachableCells.Contains(opponentPosition))
            {
                reachableCells.Remove(opponentPosition);
                reachableCells.AddRange(
                    GetCellsAvailableFromFaceToFaceSituation(playerCurrentPosition, opponentPosition));
            }

            return reachableCells;
        }

        private static PlayerNumber GetOppositePlayerNumber(PlayerNumber playerNumber)
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
        private List<CellPosition> GetCellsAvailableFromFaceToFaceSituation(
            CellPosition playerPosition, CellPosition opponentPosition)
        {
            var availableCells = new List<CellPosition>();
            int positionDifferenceX = opponentPosition.X - playerPosition.X;
            int positionDifferenceY = opponentPosition.Y - playerPosition.Y;
            // Cell behind opponent is acquired by finding next cell from player position in opponents direction
            CellPosition cellBehindOpponent = opponentPosition.Shifted(positionDifferenceX, positionDifferenceY);
            if (_field.WayBetweenExists(opponentPosition, cellBehindOpponent))
            {
                availableCells.Add(cellBehindOpponent);
            }
            else
            {
                List<CellPosition> opponentNeighbours = _field.GetReachableNeighbours(opponentPosition);
                opponentNeighbours.Remove(playerPosition);
                opponentNeighbours.Remove(cellBehindOpponent);
                availableCells.AddRange(opponentNeighbours);
            }

            return availableCells;
        }

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

        private void HandleWin(PlayerNumber winner)
        {
        }

        public void PlaceWall(PlayerNumber playerPlacing, WallPosition wallPosition)
        {
            if (!IsPlayersTurn(playerPlacing))
                throw new AnotherPlayerTurnException($"It is player {_currentPlayer} turn");

            if (!PlayerHasWalls(playerPlacing))
                throw new NoWallsLeftException($"Player {playerPlacing} has no walls left");

            _field.PlaceWall(wallPosition);
            if (!BothPlayersHaveWayToLastLine())
            {
                _field.RemoveWall(wallPosition);
                var wallDirection = wallPosition.Orientation == WallOrientation.Horizontal ? "Horizontal" : "Vertical";
                throw new WallBlocksPathForPlayerException(
                    $"{wallDirection} wall at {wallPosition.TopLeftCell} blocks way for players");
            }

            DecrementPlayerWallAmount(playerPlacing);
            SwitchCurrentPlayer();
        }

        private bool PlayerHasWalls(PlayerNumber playerNumber)
        {
            int wallAmount = playerNumber == PlayerNumber.First ? Player1WallAmount : Player2WallAmount;
            return wallAmount != 0;
        }

        //TODO think if last line is good name
        private bool BothPlayersHaveWayToLastLine()
        {
            return PlayerHasWayToLastLine(PlayerNumber.First) && PlayerHasWayToLastLine(PlayerNumber.Second);
        }

        private bool PlayerHasWayToLastLine(PlayerNumber playerNumber)
        {
            CellPosition playerCell = GetPlayerPosition(playerNumber);
            CellPosition[] winLine = GetPlayerWinLine(playerNumber);
            return winLine.Any(winCell => _aStar.WayExists(playerCell, winCell, _field));
        }

        private CellPosition[] GetPlayerWinLine(PlayerNumber playerNumber)
        {
            return playerNumber == PlayerNumber.First
                ? GameConstants.Player1WinLine
                : GameConstants.Player2WinLine;
        }

        private void DecrementPlayerWallAmount(PlayerNumber playerNumber)
        {
            if (playerNumber == PlayerNumber.First)
                Player1WallAmount--;
            else
                Player2WallAmount--;
        }
    }
}

