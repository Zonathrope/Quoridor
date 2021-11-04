using System;
using System.Collections.Generic;
using Model;
using Model.DataTypes;
using Model.Internal;

namespace AIProject
{
    public class Ai
    {
        Field _bestPosition = null;
        private readonly AStar _aStar = new AStar();
        public Field Negamax(Field position, int depth, int alpha, int beta, int color)
        {
            if (depth == 0 || CheckWin(position, color))
            {
                position.PositionValue = Sev(position, color);
                return position;
            }
            var childPositions = GeneratePositions(position, color);
            //childPositions = orderPositions(childPositions);
            var value = -999;
            
            foreach (Field childPosition in childPositions)
            {
                value = Math.Max(value, -Negamax(childPosition, depth - 1, -alpha, -beta, -color).PositionValue);
                position.PositionValue = value;
                if (_bestPosition == null || _bestPosition.PositionValue < value) {
                    _bestPosition = new Field(childPosition);
                    _bestPosition.PositionValue = value;
                }
                alpha = Math.Max(alpha, value);
                if (alpha >= beta) break;
            }
            return _bestPosition;
        }

        private LinkedList<Field> OrderPositions(object childPositions)
        {
            throw new System.NotImplementedException();
        }
        private LinkedList<Field> GeneratePositions(Field position, int color)
        {
            PlayerNumber currentPlayer = color == 1 ? PlayerNumber.First : PlayerNumber.Second;
            LinkedList<Field> possiblePositions = new LinkedList<Field>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!position.PlacedWalls.Contains(new WallPosition(WallOrientation.Horizontal, new CellPosition(i,j))))
                    {
                        try
                        {
                            Field newPosition1 = new Field(position);
                            WallPosition wall1Position = new WallPosition(WallOrientation.Vertical, new CellPosition(i, j));
                            newPosition1.PlaceWall(wall1Position, currentPlayer);
                            newPosition1.Move = new PlaceWall(wall1Position);
                            possiblePositions.AddFirst(newPosition1);
                        }
                        catch (Exception e) { }
                        try
                        {
                            Field newPosition2 = new Field(position);
                            WallPosition wall2Position = new WallPosition(WallOrientation.Horizontal, new CellPosition(i,j));
                            newPosition2.PlaceWall(wall2Position, currentPlayer);
                            newPosition2.Move = new PlaceWall(wall2Position);
                            possiblePositions.AddFirst(newPosition2);
                        }
                        catch (Exception e) { }
                        
                    }
                    
                } 
            }

            foreach (var availablePositions in position.GetCellsForMove(currentPlayer))
            {
                Field newPosition = new Field(position);
                newPosition.MovePlayer(currentPlayer, availablePositions);
                newPosition.Move = new MovePlayer(availablePositions);
                possiblePositions.AddLast(newPosition);
            }

            return possiblePositions;
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

        int Sev(Field position, int color)
        {
            int player1MinLenght = 99;
            int player2MinLenght = 99;
            foreach (CellPosition winCell in GameConstants.Player1WinLine)
            {
                int lenght = _aStar.FindPath(position.Player1Position, winCell, position).Count;
                if (lenght < player1MinLenght)
                {
                    player1MinLenght = lenght;
                }
            }   
            foreach (CellPosition winCell in GameConstants.Player2WinLine)
            {
                  int lenght = _aStar.FindPath(position.Player2Position, winCell, position).Count;
                  if (lenght < player2MinLenght)
                  {
                      player2MinLenght = lenght;
                  }
            }  
            if (color == 1)
            {
                return (position.Player1WallAmount + (8 - player1MinLenght)) - (position.Player2WallAmount + (8 - player2MinLenght)); //need theory testing
            } 
            return (position.Player2WallAmount + (8 - player2MinLenght)) - (position.Player1WallAmount + (8 - player1MinLenght));
        }
    }
    
    
}