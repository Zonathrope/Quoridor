using System;
using System.Collections.Generic;
using System.Linq;
using Model.DataTypes;

namespace Model
{
    public class Field
    {
        public Move Move = new PlaceWall(new WallPosition(WallOrientation.Vertical, new CellPosition(99,99)));
        public CellPosition Player1Position { get; set; }
        public CellPosition Player2Position { get; set; }
        internal FieldCell[,] FieldMatrix => _fieldMatrix;
        private readonly FieldCell[,] _fieldMatrix = new FieldCell[GameConstants.FieldSize,GameConstants.FieldSize];
        public List<WallPosition> PlacedWalls { get; } = new ();
        
        public int Player1WallAmount { get; private set; }
        public int Player2WallAmount { get; private set; }

        public Field(Field other)
        {
            this.Move = other.Move;
            this.Player1Position = new CellPosition(other.Player1Position.X, other.Player1Position.Y);
            this.Player2Position = new CellPosition(other.Player2Position.X, other.Player2Position.Y);
            this.Player1WallAmount = other.Player1WallAmount;
            this.Player2WallAmount = other.Player2WallAmount;
            this._fieldMatrix = CopyFieldMatrix(other._fieldMatrix);
            this.PlacedWalls = new List<WallPosition>(other.PlacedWalls);
        }
        public Field()
        {
            InitFieldMatrix();
            Player1Position = GameConstants.Player1StartPosition;
            Player2Position = GameConstants.Player2StartPosition;
            Player1WallAmount = GameConstants.StartWallAmount;
            Player2WallAmount = GameConstants.StartWallAmount;
        }
        
        public bool ExecuteMove(Move move, int color)
        {
            var success = true;
            PlayerNumber player = color == 1 ? PlayerNumber.First : PlayerNumber.Second;
            switch (move)
            {
                case MovePlayer movePlayer:
                    MovePlayer(player, movePlayer.Position);
                    break;
                case PlaceWall placeWall:
                    success = PlaceWall(placeWall.Position, player);
                    break;
            }
            return success;
        }
        public void UndoMove(Move move, CellPosition playerOldPosition, int color)
        {
            PlayerNumber player = color == 1 ? PlayerNumber.First : PlayerNumber.Second;
            switch (move)
            {
                case MovePlayer movePlayer:
                    MovePlayer(player, playerOldPosition);
                    break;
                case PlaceWall placeWall:
                    RemoveWall(placeWall.Position);
                    IncrementPlayerWallAmount(player);
                    break;
            }
        }
        
        public bool ValidateWall(WallPosition wallPosition)
        {
            return !PlacedWalls.Contains(wallPosition) && !OverlapsWithPlacedWalls(wallPosition);
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
        private FieldCell[,] CopyFieldMatrix(FieldCell[,] oldField)
        {
            FieldCell[,] newMatrix = new FieldCell[9,9];
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    newMatrix[j, i] = new FieldCell(i,j);
                    
                }
            }
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    foreach (FieldCell neighbour in oldField[j,i].ReachableNeighbours)
                    {
                        newMatrix[j, i].ReachableNeighbours.Add(newMatrix[neighbour.Position.Y,neighbour.Position.X]);
                    }
                }
            }
            return newMatrix;
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
            return cellPosition.X is >= 0 and < 9 && cellPosition.Y is >= 0 and < 9;
        }

        /// <exception cref="IncorrectWallPositionException">Caller pass invalid position.</exception>
        /// <exception cref="WallPlaceTakenException">Caller tries to place wall over existing wall.</exception>
        public bool PlaceWall(WallPosition newWallPosition, PlayerNumber playerPlacing)
        {
            if (!IsValidWallPosition(newWallPosition))
            {
                return false;
                // throw new IncorrectWallPositionException(
                //     $"({newWallPosition.TopLeftCell} is not correct wall position");
            }
            if (OverlapsWithPlacedWalls(newWallPosition))
            {
                return false;
                // throw new WallPlaceTakenException(
                //     $"There is already wall at {newWallPosition.TopLeftCell}");
            }
            
            BlockWays(newWallPosition);
            PlacedWalls.Add(newWallPosition);
            
            if (!BothPlayersHaveWayToLastLine())
            {
                RemoveWall(newWallPosition);
                return false;
                // throw new WallBlocksPathForPlayerException(
                //     $"Wall at {newWallPosition.TopLeftCell} blocks way for players");
            }
            DecrementPlayerWallAmount(playerPlacing);
            return true;
        }
        public bool BothPlayersHaveWayToLastLine()
        {
        IAStar aStar = new AStar();
            CellPosition[] winLine1 = GetPlayerWinLine(PlayerNumber.First);
            CellPosition[] winLine2 = GetPlayerWinLine(PlayerNumber.Second);
            return winLine1.Any(winCell => aStar.WayExists(Player1Position, winCell, this)) && 
                   winLine2.Any(winCell => aStar.WayExists(Player2Position, winCell, this));
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

        private bool IsCellTaken(CellPosition cell)
        {
            return cell == Player1Position || cell == Player2Position;
        }
        
        public bool OverlapsWithPlacedWalls(WallPosition newWall)
        {
            if (PlacedWalls.Any(newWall.IsEqualByPlace))
                return true;
            if (newWall.Orientation == WallOrientation.Horizontal)
            {
                return newWall.TopLeftCell.X switch
                {
                    0 => PlacedWalls.Contains(new WallPosition(WallOrientation.Horizontal,
                        new CellPosition(newWall.TopLeftCell.X + 1, newWall.TopLeftCell.Y))),
                    7 => PlacedWalls.Contains(new WallPosition(WallOrientation.Horizontal,
                        new CellPosition(newWall.TopLeftCell.X - 1, newWall.TopLeftCell.Y))),
                    _ => PlacedWalls.Contains(new WallPosition(WallOrientation.Horizontal,
                             new CellPosition(newWall.TopLeftCell.X - 1, newWall.TopLeftCell.Y))) ||
                         PlacedWalls.Contains(new WallPosition(WallOrientation.Horizontal,
                             new CellPosition(newWall.TopLeftCell.X + 1, newWall.TopLeftCell.Y)))
                };
            }
            return newWall.TopLeftCell.Y switch
                {
                    0 => PlacedWalls.Contains(new WallPosition(WallOrientation.Vertical,
                        new CellPosition(newWall.TopLeftCell.X, newWall.TopLeftCell.Y + 1))),
                    7 => PlacedWalls.Contains(new WallPosition(WallOrientation.Vertical,
                        new CellPosition(newWall.TopLeftCell.X, newWall.TopLeftCell.Y - 1))),
                    _ => PlacedWalls.Contains(new WallPosition(WallOrientation.Vertical,
                             new CellPosition(newWall.TopLeftCell.X, newWall.TopLeftCell.Y - 1))) ||
                         PlacedWalls.Contains(new WallPosition(WallOrientation.Vertical,
                             new CellPosition(newWall.TopLeftCell.X, newWall.TopLeftCell.Y + 1)))
                };
        }

        private List<CellPosition> GetReachableNeighbours(CellPosition cellPosition)
        {
            return CellByPosition(cellPosition)
                .ReachableNeighbours
                .Select(cell => cell.Position)
                .ToList();
        }

        private bool WayBetweenExists(CellPosition position1, CellPosition position2)
        {
            return CellByPosition(position1)
                .ReachableNeighbours
                .Select(cell => cell.Position)
                .Contains(position2);
        }
        
        public List<CellPosition> GetCellsForMove(PlayerNumber playerNumber)
        {
            List<CellPosition> availableMoves = GetCellsForRegularMove(playerNumber);
            if (IsJumpSituation(playerNumber))
            {
                availableMoves.AddRange(GetCellsForJump(playerNumber));
            }
            return availableMoves;
        }

        private bool IsJumpSituation(PlayerNumber playerNumber)
        {
            var playerPosition = GetPlayerPosition(playerNumber);
            var opponentPosition = GetPlayerPosition(GameModel.GetOppositePlayerNumber(playerNumber));
            return GetReachableNeighbours(playerPosition).Contains(opponentPosition);
        }

        /// <returns>cells available for regular move action</returns>
        private List<CellPosition> GetCellsForRegularMove(PlayerNumber playerNumber)
        {
            var result = GetReachableNeighbours(GetPlayerPosition(playerNumber));
            var opponentPosition = GetPlayerPosition(GameModel.GetOppositePlayerNumber(playerNumber));
            if (result.Contains(opponentPosition))
                result.Remove(opponentPosition);
            return result;
        }

        /// <returns>cells available for jump</returns>
        public List<CellPosition> GetCellsForJump(PlayerNumber playerNumber)
        {
            if (!IsJumpSituation(playerNumber))
                return new List<CellPosition>();
            CellPosition playerPosition = GetPlayerPosition(playerNumber);
            CellPosition opponentPosition = GetPlayerPosition(GameModel.GetOppositePlayerNumber(playerNumber));
            List<CellPosition> jumpCells = new();
            int positionDifferenceX = opponentPosition.X - playerPosition.X;
            int positionDifferenceY = opponentPosition.Y - playerPosition.Y;
            // Cell behind opponent is acquired by finding next cell from player position in opponents direction
            CellPosition cellBehindOpponent = opponentPosition.Shifted(positionDifferenceX, positionDifferenceY);
            if (WayBetweenExists(opponentPosition, cellBehindOpponent))
            {
                jumpCells.Add(cellBehindOpponent);
            }
            else
            {
                List<CellPosition> opponentNeighbours = GetReachableNeighbours(opponentPosition);
                opponentNeighbours.Remove(playerPosition);
                opponentNeighbours.Remove(cellBehindOpponent);
                jumpCells.AddRange(opponentNeighbours);
            }  
            return jumpCells;
        }

        private CellPosition GetPlayerPosition(PlayerNumber playerNumber)
        {
            return playerNumber == PlayerNumber.First ? Player1Position : Player2Position;
        }

        private void DecrementPlayerWallAmount(PlayerNumber playerNumber)
        {
            if (playerNumber == PlayerNumber.First)
                Player1WallAmount--;
            else
                Player2WallAmount--;
        }
        private void IncrementPlayerWallAmount(PlayerNumber playerNumber)
        {
            if (playerNumber == PlayerNumber.First)
                Player1WallAmount++;
            else
                Player2WallAmount++;
        }
    }
}   