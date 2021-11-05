﻿using System;
using System.Collections.Generic;
using Model;
using Model.DataTypes;
using Model.Internal;

namespace AIProject
{
    public class Ai
    {
        public Ai(int startDepth)
        {
            _startDepth = startDepth;
        }
        private readonly AStar _aStar = new AStar();
        private readonly int _startDepth;
        public Move Negamax(Field position, int depth, int alpha, int beta, int color)
        {
            if (depth == 0 || CheckWin(position, color))
            {
                position.Move.MoveValue = Sev(position, color);
                return position.Move;
            }
            var childPositions = GeneratePositions(position, color, depth);
            //childPositions = orderPositions(childPositions);
            var value = -999;
            
            foreach (Field childPosition in childPositions)
            {
                int negamaxRes = -(Negamax(childPosition, depth - 1, -beta, -alpha, -color).MoveValue);
                if (negamaxRes > value && depth == _startDepth)
                {
                    if (childPosition.Move is MovePlayer move)
                    {
                        position.Move = new MovePlayer(move);
                    } else position.Move = new PlaceWall((PlaceWall) childPosition.Move);
                    
                }
                value = Math.Max(value, negamaxRes);
                position.Move.MoveValue = value;
                //Console.WriteLine("d:" + (depth - 1) + " v:" + negamaxRes + " m:" + childPosition.Move);
                alpha = Math.Max(alpha, value);
                if (alpha >= beta) break;
            }
            return position.Move;
        }

        private LinkedList<Field> OrderPositions(object childPositions)
        {
            throw new System.NotImplementedException();
        }
        private LinkedList<Field> GeneratePositions(Field position, int color, int depth)
        {
            
            PlayerNumber currentPlayer = color == 1 ? PlayerNumber.First : PlayerNumber.Second;
            LinkedList<Field> possiblePositions = new LinkedList<Field>();
            
            foreach (var availablePositions in position.GetCellsForMove(currentPlayer))
            {
                Field newPosition = new Field(position);
                newPosition.MovePlayer(currentPlayer, availablePositions);
                newPosition.Move = new MovePlayer(availablePositions);
                possiblePositions.AddLast(newPosition);
                //possiblePositions.AddFirst(newPosition);
            }
            if (depth == _startDepth)
            {
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
                                possiblePositions.AddLast(newPosition1);
                            }
                            catch (Exception e) { }
                            try
                            {
                                Field newPosition2 = new Field(position);
                                WallPosition wall2Position = new WallPosition(WallOrientation.Horizontal, new CellPosition(i,j));
                                newPosition2.PlaceWall(wall2Position, currentPlayer);
                                newPosition2.Move = new PlaceWall(wall2Position);
                                possiblePositions.AddLast(newPosition2);
                            }
                            catch (Exception e) { }
                            
                        }
                        
                    } 
                }
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

        public int Sev(Field position, int color)
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