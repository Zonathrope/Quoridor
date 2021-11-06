using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Model.DataTypes;

namespace AIProject
{
    public class AI3
    {
         public AI3(int startDepth)
        {
            _startDepth = startDepth;
        }
        private readonly AStar _aStar = new AStar();
        private readonly int _startDepth;
        private readonly HashSet<ttEntry> _entryTable = new HashSet<ttEntry>();
        public Move Negascout(Field position, int depth, int alpha, int beta, int color)
        {
            var alphaOrig = alpha;
            ttEntry entry = TranspositionTableLookup(position);
            if (entry != null && entry.Depth >= depth)
            {
                switch (entry.Flag)
                {
                    case EntryFlag.EXACT:
                        return entry.Value;
                    case EntryFlag.LOWERBOUND:
                        alpha = Math.Max(alpha, entry.Value.MoveValue);
                        break;
                    case EntryFlag.UPPERBOUND:
                        beta = Math.Min(beta, entry.Value.MoveValue);
                        break;
                }
                if (alpha >= beta)
                {
                    return entry.Value;
                }
                
            }
            
            if (depth == 0 || CheckWin(position, color))
            {
                position.Move.MoveValue = Sev(position, color);
                return position.Move;
            }
            var childPositions = GeneratePositions(position, color, depth);
            var counter = 0;
            int negamaxRes = 0;
            foreach (Field childPosition in childPositions)
            {
                if (counter == 0)
                {
                    negamaxRes = -(Negascout(childPosition, depth - 1, -beta, -alpha, -color).MoveValue); 
                }
                else
                {
                    negamaxRes = -(Negascout(childPosition, depth - 1, -alpha - 1, -alpha, -color).MoveValue);
                    if (negamaxRes > alpha && negamaxRes < beta)
                    {
                        negamaxRes = -(Negascout(childPosition, depth - 1, -beta, -alpha, -color).MoveValue); 
                    }
                }
                if (negamaxRes > alpha && depth == _startDepth)
                {
                    if (childPosition.Move is MovePlayer move)
                    {
                        position.Move = new MovePlayer(move);
                    } else position.Move = new PlaceWall((PlaceWall) childPosition.Move);
                }
                alpha = Math.Max(alpha, negamaxRes);
                position.Move.MoveValue = alpha;
                
                //Console.WriteLine("d:" + (depth - 1) + " v:" + negamaxRes + " m:" + childPosition.Move);
                counter++;
                
                alpha = Math.Max(alpha, negamaxRes);
                if (alpha >= beta) break;
            }

            if (entry != null)
            {
                entry.Value = position.Move;
                entry.Value.MoveValue = negamaxRes;
                if (entry.Value.MoveValue <= alphaOrig)
                {
                    entry.Flag = EntryFlag.UPPERBOUND;
                }
                else if (entry.Value.MoveValue >= beta)
                {
                    entry.Flag = EntryFlag.UPPERBOUND;
                }
                else
                {
                    entry.Flag = EntryFlag.EXACT;
                }

                entry.Depth = depth;
                entry.Position = new Field(position);
                _entryTable.Add(entry);
            }

            return position.Move;
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
            }

            if (depth != _startDepth) return possiblePositions;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    WallPosition newWallPosition = new WallPosition(WallOrientation.Horizontal, new CellPosition(i, j));
                    if (!position.PlacedWalls.Contains(newWallPosition))
                    {
                        Field newPosition1 = new Field(position);
                        WallPosition wall1Position =
                            new WallPosition(WallOrientation.Vertical, new CellPosition(i, j));
                        newPosition1.PlaceWall(wall1Position, currentPlayer);
                        newPosition1.Move = new PlaceWall(wall1Position);
                        possiblePositions.AddLast(newPosition1);

                        Field newPosition2 = new Field(position);
                        WallPosition wall2Position =
                            new WallPosition(WallOrientation.Horizontal, new CellPosition(i, j));
                        newPosition2.PlaceWall(wall2Position, currentPlayer);
                        newPosition2.Move = new PlaceWall(wall2Position);
                        possiblePositions.AddLast(newPosition2);
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

        private int Sev(Field position, int color)
        {
            int player1MinLenght = GameConstants.Player1WinLine.Select(winCell => 
                _aStar.FindPath(position.Player1Position, winCell, position).Count).Prepend(99).Min();
            int player2MinLenght = GameConstants.Player2WinLine.Select(winCell => 
                _aStar.FindPath(position.Player2Position, winCell, position).Count).Prepend(99).Min();
            if (color == 1)
            {
                return (position.Player1WallAmount + (8 - player1MinLenght)) - (position.Player2WallAmount + (8 - player2MinLenght)); //need theory testing
            } 
            return (position.Player2WallAmount + (8 - player2MinLenght)) - (position.Player1WallAmount + (8 - player1MinLenght));
        }

        private ttEntry TranspositionTableLookup(Field position)
        {
            return _entryTable.FirstOrDefault(entry => entry.Position.Equals(position));
        }
    }
}
