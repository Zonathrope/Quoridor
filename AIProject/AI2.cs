using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Model.DataTypes;

namespace AIProject
{
    public class AI2
    {
        public AI2(int startDepth)
        {
            _startDepth = startDepth;
        }
        private readonly AStar _aStar = new AStar();
        private readonly int _startDepth;
        private const int StartAlpha = -999;
        private const int StartBeta = 999;
        private bool _firstTurn;
        public Move GetMove(Field position, PlayerNumber playerNumber, bool firstTurn)
        {
            _firstTurn = firstTurn;
            int color = PlayerToColor(playerNumber);
            return Negascout(position, _startDepth, StartAlpha, StartBeta, color);
        }
        private int PlayerToColor(PlayerNumber playerNumber)
        {
            return playerNumber == PlayerNumber.First ? 1 : -1;
        }
        public Move Negascout(Field position, int depth, int alpha, int beta, int color)
        {
            if (depth == 0 || CheckWin(position, color))
            {
                position.Move.MoveValue = Sev(position, color);
                return position.Move;
            }
            var childPositions = GeneratePositions(position, color);
            var counter = 0;
            foreach (Move childPosition in childPositions)
            {
                int negamaxRes;
                var playerCurrenPosition = color == 1 ? position.Player1Position : position.Player2Position;
                var succeeded = position.ExecuteMove(childPosition, color);
                if (!succeeded) break;
                
                if (counter == 0)
                {
                    negamaxRes = -(Negascout(position, depth - 1, -beta, -alpha, -color).MoveValue); 
                }
                else
                {
                    negamaxRes = -(Negascout(position, depth - 1, -alpha - 1, -alpha, -color).MoveValue);
                    if (negamaxRes > alpha && negamaxRes < beta)
                    {
                        negamaxRes = -(Negascout(position, depth - 1, -beta, -negamaxRes, -color).MoveValue); 
                    }
                }
                position.UndoMove(childPosition, playerCurrenPosition, color);
                
                if (negamaxRes > alpha && depth == _startDepth)
                {
                    if (childPosition is MovePlayer move)
                    {
                        position.Move = new MovePlayer(move);
                    } else position.Move = new PlaceWall((PlaceWall) childPosition);
                }
                //Console.WriteLine("d:" + (depth - 1) + " v:" + negamaxRes + " m:" + childPosition);
                
                counter++;
                alpha = Math.Max(alpha, negamaxRes);
                position.Move.MoveValue = alpha;
                if (alpha >= beta) break;
            }
            return position.Move;
        }
        
        private LinkedList<Move> GeneratePositions(Field position, int color)
        {
            PlayerNumber currentPlayer = color == 1 ? PlayerNumber.First : PlayerNumber.Second;
            LinkedList<Move> possiblePositions = new LinkedList<Move>();
            
            foreach (var newPosition in 
                position.GetCellsForMove(currentPlayer).Select(availablePositions => new MovePlayer(availablePositions)))
            {
                possiblePositions.AddLast(newPosition);
            }

            if (_firstTurn) return possiblePositions;
            if ((currentPlayer != PlayerNumber.First || position.Player1WallAmount <= 0) &&
                (currentPlayer != PlayerNumber.Second || position.Player2WallAmount <= 0)) return possiblePositions;
            
            var aroundPlayer1Position = 
                new WallPosition(WallOrientation.Horizontal, 
                    new CellPosition(position.Player1Position.X, position.Player1Position.Y));
            var aroundPlayer1Position1 =
                new WallPosition(WallOrientation.Horizontal, 
                    new CellPosition(position.Player1Position.X -1, position.Player1Position.Y));
            var aroundPlayer1Position2 =
                new WallPosition(WallOrientation.Horizontal, 
                    new CellPosition(position.Player1Position.X, position.Player1Position.Y -1));
            var aroundPlayer1Position3 =
                new WallPosition(WallOrientation.Horizontal, 
                    new CellPosition(position.Player1Position.X -1, position.Player1Position.Y-1));
            var aroundPlayer1Position4 =
                new WallPosition(WallOrientation.Vertical, 
                    new CellPosition(position.Player1Position.X, position.Player1Position.Y));
            var aroundPlayer1Position5 =
                new WallPosition(WallOrientation.Vertical, 
                    new CellPosition(position.Player1Position.X -1, position.Player1Position.Y));
            var aroundPlayer1Position6 =
                new WallPosition(WallOrientation.Vertical, 
                    new CellPosition(position.Player1Position.X, position.Player1Position.Y -1));
            var aroundPlayer1Position7 =
                new WallPosition(WallOrientation.Vertical, 
                    new CellPosition(position.Player1Position.X -1, position.Player1Position.Y-1));
            
            var aroundPlayer2Position =
                new WallPosition(WallOrientation.Horizontal, 
                    new CellPosition(position.Player2Position.X, position.Player2Position.Y));
            var aroundPlayer2Position1 =
                new WallPosition(WallOrientation.Horizontal, 
                    new CellPosition(position.Player2Position.X -1, position.Player2Position.Y));
            var aroundPlayer2Position2 =
                new WallPosition(WallOrientation.Horizontal, 
                    new CellPosition(position.Player2Position.X, position.Player2Position.Y -1));
            var aroundPlayer2Position3 =
                new WallPosition(WallOrientation.Horizontal, 
                    new CellPosition(position.Player2Position.X -1, position.Player2Position.Y-1));
            var aroundPlayer2Position4 =
                new WallPosition(WallOrientation.Vertical, 
                    new CellPosition(position.Player2Position.X, position.Player2Position.Y));
            var aroundPlayer2Position5 =
                new WallPosition(WallOrientation.Vertical, 
                    new CellPosition(position.Player2Position.X -1, position.Player2Position.Y));
            var aroundPlayer2Position6 =
                new WallPosition(WallOrientation.Vertical, 
                    new CellPosition(position.Player2Position.X, position.Player2Position.Y -1));
            var aroundPlayer2Position7 =
                new WallPosition(WallOrientation.Vertical, 
                    new CellPosition(position.Player2Position.X -1, position.Player2Position.Y-1));
            
            if (currentPlayer == PlayerNumber.First)
            {
                if (position.ValidateWall(aroundPlayer2Position) && position.ValidateWall(aroundPlayer2Position4))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position4));
                }
                if (position.ValidateWall(aroundPlayer2Position1) && position.ValidateWall(aroundPlayer2Position5))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position1));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position5));
                }
                if (position.ValidateWall(aroundPlayer2Position2) && position.ValidateWall(aroundPlayer2Position6))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position2)); 
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position6));
                }
                if (position.ValidateWall(aroundPlayer2Position3) && position.ValidateWall(aroundPlayer2Position7))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position3));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position7));
                }
                
                if (position.ValidateWall(aroundPlayer1Position) && position.ValidateWall(aroundPlayer1Position4))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position4));
                }
                if (position.ValidateWall(aroundPlayer1Position1) && position.ValidateWall(aroundPlayer1Position5))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position1));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position5));
                }
                if (position.ValidateWall(aroundPlayer1Position2) && position.ValidateWall(aroundPlayer1Position6))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position2)); 
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position6));
                }
                if (position.ValidateWall(aroundPlayer1Position3) && position.ValidateWall(aroundPlayer1Position7))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position3));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position7));
                }
            } else {
                if (position.ValidateWall(aroundPlayer1Position) && position.ValidateWall(aroundPlayer1Position4))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position4));
                }
                if (position.ValidateWall(aroundPlayer1Position1) && position.ValidateWall(aroundPlayer1Position5))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position1));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position5));
                }
                if (position.ValidateWall(aroundPlayer1Position2) && position.ValidateWall(aroundPlayer1Position6))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position2)); 
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position6));
                }
                if (position.ValidateWall(aroundPlayer1Position3) && position.ValidateWall(aroundPlayer1Position7))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position3));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer1Position7));
                }
                
                if (position.ValidateWall(aroundPlayer2Position) && position.ValidateWall(aroundPlayer2Position4))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position4));
                }
                if (position.ValidateWall(aroundPlayer2Position1) && position.ValidateWall(aroundPlayer2Position5))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position1));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position5));
                }
                if (position.ValidateWall(aroundPlayer2Position2) && position.ValidateWall(aroundPlayer2Position6))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position2)); 
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position6));
                }
                if (position.ValidateWall(aroundPlayer2Position3) && position.ValidateWall(aroundPlayer2Position7))
                {
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position3));
                    possiblePositions.AddLast(new PlaceWall(aroundPlayer2Position7));
                }
            }

            foreach (var placedWall in position.PlacedWalls)
            {
                foreach (var wall in GenerateNearbyWalls(placedWall))
                {
                    var newWall = new PlaceWall(wall);
                    if (position.ValidateWall(wall) && !possiblePositions.Contains(newWall))
                    {
                        possiblePositions.AddLast(newWall);
                    }
                }
            }

            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var newWallPosition = new WallPosition(WallOrientation.Horizontal, new CellPosition(i, j));
                    var newWallPosition2 = new WallPosition(WallOrientation.Vertical, new CellPosition(i, j));

                    var newWall = new PlaceWall(newWallPosition);
                    var newWall2 = new PlaceWall(newWallPosition2);
                    if (position.ValidateWall(newWallPosition) && position.ValidateWall(newWallPosition2) &&
                        !possiblePositions.Contains(newWall) && 
                        !possiblePositions.Contains(newWall2)) 
                    {
                        possiblePositions.AddLast(newWall);
                        possiblePositions.AddLast(newWall2); 
                    }
                }
            }
            return possiblePositions;
        }

        LinkedList<WallPosition> GenerateNearbyWalls(WallPosition wallPosition)
        {
            LinkedList<WallPosition> wallPositions = new LinkedList<WallPosition>();
            if (wallPosition.Orientation == WallOrientation.Horizontal)
            {
                var nearbyWall =
                    new WallPosition(WallOrientation.Horizontal, 
                        new CellPosition(wallPosition.TopLeftCell.X - 2, wallPosition.TopLeftCell.Y));
                var nearbyWall1 =
                    new WallPosition(WallOrientation.Horizontal, 
                        new CellPosition(wallPosition.TopLeftCell.X + 2, wallPosition.TopLeftCell.Y));
                var nearbyWall2 =
                    new WallPosition(WallOrientation.Vertical, 
                        new CellPosition(wallPosition.TopLeftCell.X - 1, wallPosition.TopLeftCell.Y - 1));
                var nearbyWall3 =
                    new WallPosition(WallOrientation.Vertical, 
                        new CellPosition(wallPosition.TopLeftCell.X + 1, wallPosition.TopLeftCell.Y + 1));
                var nearbyWall4 =
                    new WallPosition(WallOrientation.Vertical, 
                        new CellPosition(wallPosition.TopLeftCell.X - 1, wallPosition.TopLeftCell.Y + 1));
                var nearbyWall5 =
                    new WallPosition(WallOrientation.Vertical, 
                        new CellPosition(wallPosition.TopLeftCell.X + 1, wallPosition.TopLeftCell.Y - 1));
                var nearbyWall6 =
                    new WallPosition(WallOrientation.Vertical, 
                        new CellPosition(wallPosition.TopLeftCell.X - 1, wallPosition.TopLeftCell.Y));
                var nearbyWall7 =
                    new WallPosition(WallOrientation.Vertical, 
                        new CellPosition(wallPosition.TopLeftCell.X, wallPosition.TopLeftCell.Y - 1));
                wallPositions.AddLast(nearbyWall);
                wallPositions.AddLast(nearbyWall1);
                wallPositions.AddLast(nearbyWall2);
                wallPositions.AddLast(nearbyWall3);
                wallPositions.AddLast(nearbyWall4);
                wallPositions.AddLast(nearbyWall5);
                wallPositions.AddLast(nearbyWall6);
                wallPositions.AddLast(nearbyWall7);
            } else {
                var nearbyWall =
                    new WallPosition(WallOrientation.Vertical, 
                        new CellPosition(wallPosition.TopLeftCell.X - 2, wallPosition.TopLeftCell.Y));
                var nearbyWall1 =
                    new WallPosition(WallOrientation.Vertical, 
                        new CellPosition(wallPosition.TopLeftCell.X + 2, wallPosition.TopLeftCell.Y));
                var nearbyWall2 =
                    new WallPosition(WallOrientation.Horizontal, 
                        new CellPosition(wallPosition.TopLeftCell.X - 1, wallPosition.TopLeftCell.Y - 1));
                var nearbyWall3 =
                    new WallPosition(WallOrientation.Horizontal, 
                        new CellPosition(wallPosition.TopLeftCell.X + 1, wallPosition.TopLeftCell.Y + 1));
                var nearbyWall4 =
                    new WallPosition(WallOrientation.Horizontal, 
                        new CellPosition(wallPosition.TopLeftCell.X - 1, wallPosition.TopLeftCell.Y + 1));
                var nearbyWall5 =
                    new WallPosition(WallOrientation.Horizontal, 
                        new CellPosition(wallPosition.TopLeftCell.X + 1, wallPosition.TopLeftCell.Y - 1));
                var nearbyWall6 =
                    new WallPosition(WallOrientation.Horizontal, 
                        new CellPosition(wallPosition.TopLeftCell.X - 1, wallPosition.TopLeftCell.Y));
                var nearbyWall7 =
                    new WallPosition(WallOrientation.Horizontal, 
                        new CellPosition(wallPosition.TopLeftCell.X, wallPosition.TopLeftCell.Y - 1));
                wallPositions.AddLast(nearbyWall);
                wallPositions.AddLast(nearbyWall1);
                wallPositions.AddLast(nearbyWall2);
                wallPositions.AddLast(nearbyWall3);
                wallPositions.AddLast(nearbyWall4);
                wallPositions.AddLast(nearbyWall5);
                wallPositions.AddLast(nearbyWall6);
                wallPositions.AddLast(nearbyWall7);
            }
            return wallPositions;
        }

        bool CheckWin(Field position, int color)
        {
            CellPosition playerPosition = color == 1 ? position.Player1Position : position.Player2Position;
            if (color == 1)
            {
                return playerPosition.Y == 0;
            }
            return playerPosition.Y == 8;
        }

        private int Sev(Field position, int color)
        {
            var player1MinLenght = GameConstants.Player1WinLine.Select(winCell => 
                _aStar.FindPath(position.Player1Position, winCell, position).Count).Prepend(999).Min();

            var player2MinLenght = GameConstants.Player2WinLine.Select(winCell => 
                _aStar.FindPath(position.Player2Position, winCell, position).Count).Prepend(999).Min();

            if (color == 1)
            {
                switch (player1MinLenght)
                {
                    case 0 when player2MinLenght == 0:
                        return 0;
                    case 0:
                        return 99;
                    default:
                    {
                        if (player2MinLenght == 0)
                        {
                            return -99;
                        }
                        break;
                    }
                }
                return (position.Player1WallAmount + (8 - player1MinLenght)*2) - (position.Player2WallAmount + (8 - player2MinLenght)*2);
            }
            
            switch (player1MinLenght)
            {
                case 0 when player2MinLenght == 0:
                    return 0;
                case 0:
                    return -99;
                default:
                {
                    if (player2MinLenght == 0)
                    {
                        return 99;
                    }
                    break;
                }
            }
            return (position.Player2WallAmount + (8 - player2MinLenght)*2) - (position.Player1WallAmount + (8 - player1MinLenght)*2);
        }
    }
}