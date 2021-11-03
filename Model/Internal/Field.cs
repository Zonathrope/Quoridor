using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using Model.DataTypes;

namespace Model.Internal
{
    public class Field
    {
        public int PositionValue;
        public Move Move;
        public CellPosition Player1Position { get; private set; }
        public CellPosition Player2Position { get; private set; }
        internal FieldCell[,] FieldMatrix => _fieldMatrix;
        private FieldCell[,] _fieldMatrix = new FieldCell[GameConstants.FieldSize,GameConstants.FieldSize];
        private IAStar _aStar = new AStar();
        public List<WallPosition> PlacedWalls { get; } = new ();
        
        public int Player1WallAmount { get; private set; }
        public int Player2WallAmount { get; private set; }

        public Field(Field other) {
            this.Move = other.Move;
            this.Player1Position = other.Player1Position;
            this.Player2Position = other.Player2Position;
            this._fieldMatrix = other._fieldMatrix;
            this.PlacedWalls = PlacedWalls;
        }
        public Field()
        {
            InitFieldMatrix();
            Player1Position = GameConstants.Player1StartPosition;
            Player2Position = GameConstants.Player2StartPosition;
            Player1WallAmount = GameConstants.StartWallAmount;
            Player2WallAmount = GameConstants.StartWallAmount;
        }

        private void InitFieldMatrix()
        {
            for (int y = 0; y < _fieldMatrix.GetLength(0); y++)
            {
                for (int x = 0; x < _fieldMatrix.GetLength(1); x++)
                {
                    _fieldMatrix[y, x] = new FieldCell(x, y);
                }
            }
            foreach (var cell in _fieldMatrix)
            {
                AddNeighboursToCell(cell);
            }
        }

        /// <summary>Add cell neighbours as reachable neighbours</summary>
        private void AddNeighboursToCell(FieldCell cell)
        {
            GetCellNeighbours(cell).ForEach(cell.AddReachableNeighbour);
        }

        private List<FieldCell> GetCellNeighbours(FieldCell cell)
        {
            var neighbours = new List<FieldCell>();
            CellPosition cellPosition = cell.Position;
            // checks to account for field borders
            if (cellPosition.Y != 0)
            {
                neighbours.Add(CellByPosition(cellPosition.Shifted(0, -1)));
            }
            if (cellPosition.X != 8)
            {
                neighbours.Add(CellByPosition(cellPosition.Shifted(1, 0)));
            }
            if (cellPosition.Y != 8)
            {
                neighbours.Add(CellByPosition(cellPosition.Shifted(0, 1)));
            }
            if (cellPosition.X != 0)
            {
                neighbours.Add(CellByPosition(cellPosition.Shifted(-1, 0)));
            }
            return neighbours;
        }

        public FieldCell CellByPosition(CellPosition cellPosition)
        {
            return _fieldMatrix[cellPosition.Y, cellPosition.X];
        }

        /// <exception cref="IncorrectPlayerPositionException">Caller pass invalid position.</exception>
        /// <exception cref="CellAlreadyTakenException">Caller tries to move to taken cell.</exception>
        public void MovePlayer(PlayerNumber playerNumber, CellPosition cellPosition)
        {
            if (!IsOnField(cellPosition))
                throw new IncorrectPlayerPositionException($"({cellPosition} is not on field");

            if (IsCellTaken(cellPosition))
            {
                throw new CellAlreadyTakenException(
                    $"Player {playerNumber} can't take cell ${cellPosition}, it is already taken by {playerNumber}");
            }
            SetPlayerPosition(playerNumber, cellPosition);
        }

        private void SetPlayerPosition(PlayerNumber playerNumber, CellPosition cellPosition)
        {
            if (playerNumber == PlayerNumber.First)
            {
                Player1Position = cellPosition;
            }
            else
            {
                Player2Position = cellPosition;
            }
        }

        private static bool IsOnField(CellPosition cellPosition)
        {
            return IsInFieldCoordinatesRange(cellPosition.X) && IsInFieldCoordinatesRange(cellPosition.Y);
        }

        private static bool IsInFieldCoordinatesRange(int value)
        {
            return value is >= 0 and < 9;
        }

        /// <exception cref="IncorrectWallPositionException">Caller pass invalid position.</exception>
        /// <exception cref="WallPlaceTakenException">Caller tries to place wall over existing wall.</exception>
        public void PlaceWall(WallPosition newWallPosition, PlayerNumber playerPlacing)
        {
            if (!IsValidWallPosition(newWallPosition))
            {
                throw new IncorrectWallPositionException(
                    $"({newWallPosition.TopLeftCell} is not correct wall position");
            }
            if (PlacedWalls.Any(newWallPosition.IsEqualByPlace))
            {
                throw new WallPlaceTakenException(
                    $"There is already wall at {newWallPosition.TopLeftCell}");
            }
            BlockWays(newWallPosition);
            if (!BothPlayersHaveWayToLastLine())
            {
                RemoveWall(newWallPosition);
                throw new WallBlocksPathForPlayerException(
                    $"Wall at {newWallPosition.TopLeftCell} blocks way for players");
            }
            DecrementPlayerWallAmount(playerPlacing);
            PlacedWalls.Add(newWallPosition);
        }
        private bool BothPlayersHaveWayToLastLine()
        {
            CellPosition[] winLine1 = GetPlayerWinLine(PlayerNumber.First);
            CellPosition[] winLine2 = GetPlayerWinLine(PlayerNumber.Second);
            return winLine1.Any(winCell => _aStar.WayExists(Player1Position, winCell, this)) && 
                   winLine2.Any(winCell => _aStar.WayExists(Player2Position, winCell, this));
        }
        
        private CellPosition[] GetPlayerWinLine(PlayerNumber playerNumber)
        {
            return playerNumber == PlayerNumber.First
                ? GameConstants.Player1WinLine
                : GameConstants.Player2WinLine;
        }

        //TODO ask if it is ok that exception is documented here, but not in calling method
        /// <exception cref="NoSuchWallException">passed wall wasn't placed.</exception>
        public void RemoveWall(WallPosition wallPosition)
        {
            if (!PlacedWalls.Contains(wallPosition))
                throw new NoSuchWallException($"There is no {wallPosition} among placed walls");
            PlacedWalls.Remove(wallPosition);
            RestoreWays(removedWall:wallPosition);
        }

        private void BlockWays(WallPosition newWall)
        {
            if (newWall.Orientation == WallOrientation.Horizontal)
            {
                BlockWayBetweenCells(_fieldMatrix[newWall.TopLeftCell.Y, newWall.TopLeftCell.X], 
                    _fieldMatrix[newWall.TopLeftCell.Y + 1, newWall.TopLeftCell.X]);
                BlockWayBetweenCells(_fieldMatrix[newWall.TopLeftCell.Y, newWall.TopLeftCell.X + 1], 
                    _fieldMatrix[newWall.TopLeftCell.Y + 1, newWall.TopLeftCell.X + 1]);
            }
            else
            {
                BlockWayBetweenCells(_fieldMatrix[newWall.TopLeftCell.Y, newWall.TopLeftCell.X], 
                    _fieldMatrix[newWall.TopLeftCell.Y, newWall.TopLeftCell.X + 1]);
                BlockWayBetweenCells(_fieldMatrix[newWall.TopLeftCell.Y + 1, newWall.TopLeftCell.X], 
                    _fieldMatrix[newWall.TopLeftCell.Y + 1, newWall.TopLeftCell.X + 1]);
            }
        }

        private void RestoreWays(WallPosition removedWall)
        {
            if (removedWall.Orientation == WallOrientation.Horizontal)
            {
                RestoreWayBetweenCells(_fieldMatrix[removedWall.TopLeftCell.Y, removedWall.TopLeftCell.X], 
                    _fieldMatrix[removedWall.TopLeftCell.Y + 1, removedWall.TopLeftCell.X]);
                RestoreWayBetweenCells(_fieldMatrix[removedWall.TopLeftCell.Y, removedWall.TopLeftCell.X + 1], 
                    _fieldMatrix[removedWall.TopLeftCell.Y + 1, removedWall.TopLeftCell.X + 1]);
            }
            else
            {
                RestoreWayBetweenCells(_fieldMatrix[removedWall.TopLeftCell.Y, removedWall.TopLeftCell.X], 
                    _fieldMatrix[removedWall.TopLeftCell.Y, removedWall.TopLeftCell.X + 1]);
                RestoreWayBetweenCells(_fieldMatrix[removedWall.TopLeftCell.Y + 1, removedWall.TopLeftCell.X], 
                    _fieldMatrix[removedWall.TopLeftCell.Y + 1, removedWall.TopLeftCell.X + 1]);
            }
        }
        private void BlockWayBetweenCells(FieldCell cell1, FieldCell cell2)
        {
            cell1.RemoveReachableNeighbour(cell2);
            cell2.RemoveReachableNeighbour(cell1);
        }

        private void RestoreWayBetweenCells(FieldCell cell1, FieldCell cell2)
        {
            cell1.AddReachableNeighbour(cell2);
            cell2.AddReachableNeighbour(cell1);
        }

        private bool IsValidWallPosition(WallPosition wallPosition)
        {
            CellPosition bottomRightCell;
            //Exception will fire if top left cell is on field bottom or right edge
            try
            {
                bottomRightCell = wallPosition.TopLeftCell.Shifted(1, 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
            return IsOnField(wallPosition.TopLeftCell) && IsOnField(bottomRightCell);
        }

        public bool IsCellTaken(CellPosition cell)
        {
            return cell == Player1Position || cell == Player2Position;
        }

        public List<CellPosition> GetReachableNeighbours(CellPosition cellPosition)
        {
            return CellByPosition(cellPosition)
                .ReachableNeighbours
                .Select(cell => cell.Position)
                .ToList();
        }

        public bool WayBetweenExists(CellPosition position1, CellPosition position2)
        {
            return CellByPosition(position1)
                .ReachableNeighbours
                .Select(cell => cell.Position)
                .Contains(position2);
        }
        
        public List<CellPosition> GetCellsForMove(PlayerNumber playerNumber)
        {
            CellPosition playerCurrentPosition = playerNumber == PlayerNumber.First ? Player1Position : Player2Position;
            CellPosition opponentPosition = playerNumber == PlayerNumber.First ? Player2Position : Player1Position;
            List<CellPosition> reachableCells = GetReachableNeighbours(playerCurrentPosition);
            if (IsJumpSituation(reachableCells, opponentPosition))
            {
                return GetCellsForJump(playerNumber);
            }
            return reachableCells;
        }

        public bool IsJumpSituation(List<CellPosition> reachableCells, CellPosition opponentPosition)
        {
            return reachableCells.Contains(opponentPosition);
        }
        private List<CellPosition> GetCellsForJump(PlayerNumber playerNumber)
        {
            CellPosition playerPosition = playerNumber == PlayerNumber.First ? Player1Position : Player2Position;
            CellPosition opponentPosition = playerNumber == PlayerNumber.First ? Player2Position : Player1Position;
            List<CellPosition> reachableCells = GetReachableNeighbours(playerPosition);
            reachableCells.Remove(opponentPosition);
            var availableCells = new List<CellPosition>();
            int positionDifferenceX = opponentPosition.X - playerPosition.X;
            int positionDifferenceY = opponentPosition.Y - playerPosition.Y;
            // Cell behind opponent is acquired by finding next cell from player position in opponents direction
            CellPosition cellBehindOpponent = opponentPosition.Shifted(positionDifferenceX, positionDifferenceY);
            if (WayBetweenExists(opponentPosition, cellBehindOpponent))
            {
                availableCells.Add(cellBehindOpponent);
            }
            else
            {
                List<CellPosition> opponentNeighbours = GetReachableNeighbours(opponentPosition);
                opponentNeighbours.Remove(playerPosition);
                opponentNeighbours.Remove(cellBehindOpponent);
                availableCells.AddRange(opponentNeighbours);
            }

            reachableCells.AddRange(availableCells);
            return reachableCells;
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