using System;
using System.Collections.Generic;
using System.Linq;
using Model.Api;
using Model.DataTypes;

namespace Model
{
    class Field
    {
        private class CellsAroundWall
        {
            public FieldCell TopLeftCell     { get; }
            public FieldCell TopRightCell    { get; }
            public FieldCell BottomRightCell { get; }
            public FieldCell BottomLeftCell  { get; }

            public CellsAroundWall(FieldCell topLeftCell, FieldCell topRightCell,
                                   FieldCell bottomRightCell, FieldCell bottomLeftCell)
            {
                TopLeftCell = topLeftCell;
                TopRightCell = topRightCell;
                BottomRightCell = bottomRightCell;
                BottomLeftCell = bottomLeftCell;
            }
        }
        public int Size => FieldSize;
        private const int FieldSize = 9;
        public int FieldMiddleCoordinate => _fieldMiddleCordinat;
        private const int _fieldMiddleCordinat = 4;
        private const int PlayerWallAmount = 10;
        private readonly CellPosition Player1DefaultPosition = new CellPosition(_fieldMiddleCordinat, FieldSize - 1);
        private readonly CellPosition Player2DefaultPosition = new CellPosition(_fieldMiddleCordinat, 0);

        public FieldCell[,] FieldMatrix => _fieldMatrix;
        private FieldCell[,] _fieldMatrix = new FieldCell[FieldSize,FieldSize];
        public List<WallPosition> PlacedWalls => _placedWalls;
        private List<WallPosition> _placedWalls = new List<WallPosition>();
        public int Player1WallAmount { set; get; } = PlayerWallAmount;
        public int Player2WallAmount { set; get; } = PlayerWallAmount;

        public CellPosition Player1Position { set; get; }
        public CellPosition Player2Position { set; get; }
        public Field()
        {
            InitMatrix();
            Player1Position = Player1DefaultPosition;
            Player2Position = Player2DefaultPosition;
        }

        private void InitMatrix()
        {
            for (int y = 0; y < FieldSize; y++)
            {
                for (int x = 0; x < FieldSize; x++)
                {
                    _fieldMatrix[y, x] = new FieldCell(x, y);
                }
            }
            for (int y = 0; y < FieldSize; y++)
            {
                for (int x = 0; x < FieldSize; x++)
                {
                    FieldCell cell = CellByPosition(new CellPosition(x, y));
                    foreach (FieldCell neighbour in GetCellNeighbours(cell))
                    {
                        cell.AddReachableNeighbour(neighbour);
                    }
                }
            }
        }
        private List<FieldCell> GetCellNeighbours(FieldCell cell)
        {
            List<FieldCell> neighbours = new List<FieldCell>();
            CellPosition cellPosition = cell.Position;
            if (cellPosition.Y != 0)
            {
                //TODO think if it is okay what cell positon can accept incorrect values
                neighbours.Add(CellByPosition(cellPosition.Shifted(0, -1)));
            }
            if (cellPosition.X != (FieldSize - 1))
            {
                neighbours.Add(CellByPosition(cellPosition.Shifted(1, 0)));
            }
            if (cellPosition.Y != (FieldSize - 1))
            {
                neighbours.Add(CellByPosition(cellPosition.Shifted(0, 1)));
            }
            if (cellPosition.X != 0)
            {
                neighbours.Add(CellByPosition(cellPosition.Shifted(-1, 0)));
            }
            return neighbours;
        }

        private FieldCell CellByPosition(CellPosition cellPosition)
        {
            return _fieldMatrix[cellPosition.Y, cellPosition.X];
        }

        /// <exception cref="IncorrectPlayerPositionException">Caller pass invalid position.</exception>
        /// <exception cref="CellAlreadyTakenException">Caller tries to move to taken cell.</exception>
        public void MovePlayer(PlayerNumber playerNumber, CellPosition position)
        {
            if (!IsOnField(position))
            {
                throw new IncorrectPlayerPositionException($"({position} is not on field");
            }

            if ((playerNumber == PlayerNumber.First && position == Player2Position) ||
                 playerNumber == PlayerNumber.Second && position == Player1Position)
            {
                throw new CellAlreadyTakenException(
                    $"Player {playerNumber} can't take cell ${position}, it is already taken by other player");
            }

            if (playerNumber == PlayerNumber.First)
            {
                Player1Position = position;
            }
            else
            {
                Player2Position = position;
            }
        }

        public bool IsOnField(CellPosition position)
        {
            return IsInFieldCoordinatesRange(position.X) && IsInFieldCoordinatesRange(position.Y);
        }
        private bool IsInFieldCoordinatesRange(int value)
        {
            return value >= 0 && value < FieldSize;
        }

        /// <exception cref="IncorrectWallPositionException">Caller pass invalid position.</exception>
        /// <exception cref="WallPlaceTakenException">Caller tries to place wall over existing wall.</exception>
        //TODO think if it is good to call every position argument just position
        public void PlaceWall(PlayerNumber playerNumber, WallPosition position)
        {
            if (!IsValidWallPosition(position))
            {
                throw new IncorrectWallPositionException(
                    $"({position.TopLeftCell}, {position.BottomRightCell}) is not correct wall position");
            }

            foreach (WallPosition placedWall in _placedWalls)
            {
                if (position.TopLeftCell == placedWall.TopLeftCell &&
                    position.BottomRightCell == placedWall.BottomRightCell)
                {
                    throw new WallPlaceTakenException(
                        $"There is already wall between {position.TopLeftCell} and {position.BottomRightCell}");
                }
            }

            //TODO think if i should have some class for player representation inside field class
            if (playerNumber == PlayerNumber.First)
            {
                if (Player1WallAmount == 0) throw new NoWallsLeftException("Player 1 have no walls left");
                //TODO think if calling private fields through property is okay
                //TODO check if it works at all with decrement
                Player1WallAmount--;
            }
            else
            {
                if (Player2WallAmount == 0) throw new NoWallsLeftException("Player 2 have no walls left");
                Player2WallAmount--;
            }
            _placedWalls.Add(position);
            BlockWaysBetweenCells(position);
        }

        public void RemoveWall(WallPosition position)
        {
            if (!_placedWalls.Contains(position)) return;
            _placedWalls.Remove(position);
            RestoreWaysBetweenCells(position);
        }

        // TODO think how to rename method
        private void BlockWaysBetweenCells(WallPosition newWall)
        {
            CellsAroundWall cells = GetCellsAroundWall(newWall);
            if (newWall.Direction == WallDirection.Horizontal)
            {
                BlockWayBetweenCells(cells.TopLeftCell, cells.TopRightCell);
                BlockWayBetweenCells(cells.BottomLeftCell, cells.BottomRightCell);
            }
            else
            {
                BlockWayBetweenCells(cells.TopLeftCell, cells.BottomLeftCell);
                BlockWayBetweenCells(cells.TopRightCell, cells.BottomRightCell);
            }
        }

        private CellsAroundWall GetCellsAroundWall(WallPosition wallPosition)
        {
            //TODO think if i really need to store two positions in wall position
            CellPosition topLeftCell = wallPosition.TopLeftCell;
            CellPosition topRightCell = wallPosition.TopLeftCell.Shifted(1, 0);
            CellPosition bottomRightCell = wallPosition.BottomRightCell;
            CellPosition bottomLeftCell = wallPosition.BottomRightCell.Shifted(-1, 0);
            return new CellsAroundWall(
                CellByPosition(topLeftCell),
                CellByPosition(topRightCell),
                CellByPosition(bottomRightCell),
                CellByPosition(bottomLeftCell));
        }

        private void RestoreWaysBetweenCells(WallPosition removedWall)
        {
            CellsAroundWall cells = GetCellsAroundWall(removedWall);
            if (removedWall.Direction == WallDirection.Horizontal)
            {
                RestoreWayBetweenCells(cells.TopLeftCell, cells.TopRightCell);
                RestoreWayBetweenCells(cells.BottomLeftCell, cells.BottomRightCell);
            }
            else
            {
                RestoreWayBetweenCells(cells.TopLeftCell, cells.BottomLeftCell);
                RestoreWayBetweenCells(cells.TopRightCell, cells.BottomRightCell);
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

        private bool IsValidWallPosition(WallPosition position)
        {
            CellPosition topLeftCell = position.TopLeftCell;
            CellPosition bottomRightCell = position.BottomRightCell;
            return IsOnField(topLeftCell) &&
                   IsOnField(bottomRightCell)
                   // check if cell is really bottom right relative to top left
                   && bottomRightCell == topLeftCell.Shifted(1, 1);
        }

        public FieldCell[] GetPlayersWinLine(PlayerNumber playerNumber)
        {
            if (playerNumber == PlayerNumber.First)
            {
                return GetFieldRow(0);
            }

            return GetFieldRow(FieldSize - 1);
        }

        private FieldCell[] GetFieldRow(int rowNumber)
        {
            return Enumerable.Range(0, FieldSize - 1)
                .Select(columnNumber => _fieldMatrix[rowNumber, columnNumber])
                .ToArray();
        }

        public FieldCell GetPlayerCell(PlayerNumber playerNumber)
        {
            return playerNumber == PlayerNumber.First
                ? CellByPosition(Player1Position)
                : CellByPosition(Player2Position);
        }

        public CellPosition GetPlayerPosition(PlayerNumber playerNumber)
        {
            return playerNumber == PlayerNumber.First
                ? Player1Position
                : Player2Position;
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

        public bool WayBetweenCellsExists(CellPosition position1, CellPosition position2)
        {
            return CellByPosition(position1)
                .ReachableNeighbours
                .Select(cell => cell.Position)
                .Contains(position2);
        }
    }
}