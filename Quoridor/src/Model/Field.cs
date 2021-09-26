using System.Collections.Generic;

namespace Quoridor.Model
{
    class Field
    {
        private const int FieldSize = 9;
        private const int PlayerWallAmount = 10;
        private static readonly CellPosition Player1DefaultPosition = new CellPosition(4, 0);
        private static readonly CellPosition Player2DefaultPosition = new CellPosition(4, 8);
    
        private FieldCell[,] _fieldMatrix = new FieldCell[FieldSize,FieldSize];
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
                        cell.AddNeighbour(neighbour);
                    }
                }
            }
        }
        // TODO think how to rename method to not conflict with fact what neighbours
        // can be removed during game flow
        private List<FieldCell> GetCellNeighbours(FieldCell cell)
        {
            List<FieldCell> neighbours = new List<FieldCell>();
            CellPosition cellPosition = cell.Position;
            if (cellPosition.Y != 0)
            {
                //TODO think if it is okay what cell positon can accept incorrect values
                neighbours.Add(CellByPosition(cellPosition + new CellPosition(0, -1)));
            }
            if (cellPosition.X != (FieldSize - 1))
            {
                neighbours.Add(CellByPosition(cellPosition + new CellPosition(1, 0)));
            }
            if (cellPosition.Y != (FieldSize - 1))
            {
                neighbours.Add(CellByPosition(cellPosition + new CellPosition(0, 1)));
            }
            if (cellPosition.X != 0)
            {
                neighbours.Add(CellByPosition(cellPosition + new CellPosition(-1, 0)));
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
            if (!IsValidCellPosition(position))
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

        private bool IsValidCellPosition(CellPosition position)
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
            UpdateWaysBetweenCells(position);
        }

        private void UpdateWaysBetweenCells(WallPosition newWall)
        {
            FieldCell cell1 = CellByPosition(newWall.TopLeftCell);
            FieldCell cell3 = CellByPosition(newWall.BottomRightCell);
            FieldCell cell2, cell4;
            if (newWall.Direction == WallDirection.Vertical)
            {
                cell2 = CellByPosition(newWall.TopLeftCell + new CellPosition(1, 0));
                cell4 = CellByPosition(newWall.BottomRightCell + new CellPosition(1, 0));
            }
            else
            {
                cell2 = CellByPosition(newWall.TopLeftCell + new CellPosition(0, 1));
                cell4 = CellByPosition(newWall.BottomRightCell + new CellPosition(0, 1));
            }
            BlockWayBetweenCells(cell1, cell2);
            BlockWayBetweenCells(cell3, cell4);
        }

        private void BlockWayBetweenCells(FieldCell cell1, FieldCell cell2)
        {
            cell1.RemoveNeighbour(cell2);
            cell2.RemoveNeighbour(cell1);
        }

        private bool IsValidWallPosition(WallPosition position)
        {
            CellPosition topLeftCell = position.TopLeftCell;
            CellPosition bottomRightCell = position.BottomRightCell;
            return IsValidCellPosition(topLeftCell) &&
                   IsValidCellPosition(bottomRightCell)
                   // check if cell is really bottom right relative to top left
                   && bottomRightCell == topLeftCell + new CellPosition(1, 1);
        }
    }
}