using System.Collections.Generic;

namespace Quoridor.Model
{
    class Field
    {
        class FieldCell
        {
            public CellPosition Position { get; }
            private List<FieldCell> _neighbourCells = new List<FieldCell>();

            public FieldCell(int x, int y)
            {
                Position = new CellPosition(x, y);
            }
            public void AddNeighbour(FieldCell neighbour)
            {
                _neighbourCells.Add(neighbour);
            }

            public void RemoveNeighbour(FieldCell neighbour)
            {
                _neighbourCells.Remove(neighbour);
            }
        }

        private const int FieldSize = 9;
        private static readonly CellPosition Player1DefaultPosition = new CellPosition(4, 0);
        private static readonly CellPosition Player2DefaultPosition = new CellPosition(4, 8);
    
        private FieldCell[,] _fieldMatrix = new FieldCell[FieldSize,FieldSize];
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

        public void MovePlayer(PlayerNumber playerNumber, CellPosition position)
        {
            if (!IsInFieldCoordinatesRange(position.X) || !IsInFieldCoordinatesRange(position.Y))
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

        private bool IsInFieldCoordinatesRange(int value)
        {
            return value >= 0 && value < FieldSize;
        }
    }
}