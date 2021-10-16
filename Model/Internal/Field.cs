using System;
using System.Collections.Generic;
using System.Linq;
using Model.DataTypes;

namespace Model.Internal
{
    class Field
    {
        private record CellsAroundWall(FieldCell TopLeftCell, FieldCell TopRightCell,
            FieldCell BottomRightCell, FieldCell BottomLeftCell);

        public CellPosition Player1Position { get; private set; }
        public CellPosition Player2Position { get; private set; }
        internal FieldCell[,] FieldMatrix => _fieldMatrix;
        private FieldCell[,] _fieldMatrix = new FieldCell[GameConstants.FieldSize,GameConstants.FieldSize];
        public List<WallPosition> PlacedWalls { get; } = new ();


        public Field()
        {
            InitFieldMatrix();
            Player1Position = GameConstants.Player1StartPosition;
            Player2Position = GameConstants.Player2StartPosition;
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
            if (cellPosition.X != GameConstants.FieldEndCoordinate)
            {
                neighbours.Add(CellByPosition(cellPosition.Shifted(1, 0)));
            }
            if (cellPosition.Y != GameConstants.FieldEndCoordinate)
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
            return value is >= 0 and < GameConstants.FieldSize;
        }

        /// <exception cref="IncorrectWallPositionException">Caller pass invalid position.</exception>
        /// <exception cref="WallPlaceTakenException">Caller tries to place wall over existing wall.</exception>
        public void PlaceWall(WallPosition newWallPosition)
        {
            if (!IsValidWallPosition(newWallPosition))
            {
                throw new IncorrectWallPositionException(
                    $"({newWallPosition.TopLeftCell} is not correct wall position");
            }
            if (OverlapsWithPlacedWalls(newWallPosition))
            {
                throw new WallPlaceTakenException(
                    $"There is already wall at {newWallPosition.TopLeftCell}");
            }

            PlacedWalls.Add(newWallPosition);
            BlockWays(newWall: newWallPosition);
        }

        // TODO overlook if it can be written shorter
        private bool OverlapsWithPlacedWalls(WallPosition newWall)
        {
            if (PlacedWalls.Any(newWall.IsEqualByPlace))
                return true;
            if (newWall.Orientation == WallOrientation.Horizontal)
            {
                foreach (WallPosition placedWall in PlacedWalls)
                {
                    CellPosition cellToRight = null;
                    CellPosition cellToLeft = null;
                    try
                    {
                        cellToRight = placedWall.TopLeftCell.Shifted(1, 0);
                        cellToLeft = placedWall.TopLeftCell.Shifted(-1, 0);
                    }
                    catch (ArgumentOutOfRangeException e){}
                    if (newWall.TopLeftCell == cellToLeft || newWall.TopLeftCell == cellToRight)
                        return true;
                }
            }
            else
            {
                foreach (WallPosition placedWall in PlacedWalls)
                {
                    CellPosition cellAbove = null;
                    CellPosition cellBelow = null;
                    try
                    {
                        cellAbove = placedWall.TopLeftCell.Shifted(0, 1);
                        cellBelow = placedWall.TopLeftCell.Shifted(0, -1);
                    }
                    catch (ArgumentOutOfRangeException e){}

                    if (newWall.TopLeftCell == cellAbove || newWall.TopLeftCell == cellBelow)
                        return true;
                }
            }

            return false;
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
            CellsAroundWall cells = GetCellsAroundWall(newWall);
            if (newWall.Orientation == WallOrientation.Horizontal)
            {
                BlockWayBetweenCells(cells.TopLeftCell, cells.BottomLeftCell);
                BlockWayBetweenCells(cells.TopRightCell, cells.BottomRightCell);
            }
            else
            {
                BlockWayBetweenCells(cells.TopLeftCell, cells.TopRightCell);
                BlockWayBetweenCells(cells.BottomLeftCell, cells.BottomRightCell);
            }
        }

        private CellsAroundWall GetCellsAroundWall(WallPosition wallPosition)
        {
            CellPosition topLeftCell     = wallPosition.TopLeftCell;
            CellPosition topRightCell    = topLeftCell.Shifted(1, 0);
            CellPosition bottomRightCell = topLeftCell.Shifted(1, 1);
            CellPosition bottomLeftCell  = topLeftCell.Shifted(0, 1);
            return new CellsAroundWall(
                CellByPosition(topLeftCell),
                CellByPosition(topRightCell),
                CellByPosition(bottomRightCell),
                CellByPosition(bottomLeftCell));
        }

        private void RestoreWays(WallPosition removedWall)
        {
            CellsAroundWall cells = GetCellsAroundWall(removedWall);
            if (removedWall.Orientation == WallOrientation.Horizontal)
            {
                RestoreWayBetweenCells(cells.TopLeftCell, cells.BottomLeftCell);
                RestoreWayBetweenCells(cells.TopRightCell, cells.BottomRightCell);
            }
            else
            {
                RestoreWayBetweenCells(cells.TopLeftCell, cells.TopRightCell);
                RestoreWayBetweenCells(cells.BottomLeftCell, cells.BottomRightCell);
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
            CellPosition topLeftCell = wallPosition.TopLeftCell;
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
            return IsOnField(topLeftCell) && IsOnField(bottomRightCell);
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
    }
}   