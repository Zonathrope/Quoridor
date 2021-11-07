using System;
using System.Collections.Generic;
using Model;
using Model.DataTypes;

namespace AIProject
{
    public class Ai
    {
        private readonly AStar _aStar = new AStar();
        private readonly int _startDepth;
        private const int StartAlpha = -999;
        private const int StartBeta = 999;

        public Ai(int startDepth)
        {
            _startDepth = startDepth;
        }

        public Move GetMove(Field position, PlayerNumber playerNumber)
        {
            int color = PlayerToColor(playerNumber);
            return Negamax(position, _startDepth, StartAlpha, StartBeta, color);
        }

        private int PlayerToColor(PlayerNumber playerNumber)
        {
            return playerNumber == PlayerNumber.First ? 1 : -1;
        }
        public Move Negamax(Field position, int depth, int alpha, int beta, int color)
        {
            if (depth == 0 || CheckWin(position, color))
            {
                position.Move.MoveValue = Sev(position, color);
                return position.Move;
            }
            var childPositions = GeneratePositions(position, color, depth);
            var value = -99;
            
            foreach (Move childPosition in childPositions)
            {
                int negamaxRes = 0;
                try
                {
                    var playerCurrenPosition = color == 1 ? position.Player1Position : position.Player2Position;
                    ExecuteMove(childPosition,position, color);
                    negamaxRes = -(Negamax(position, depth - 1, -beta, -alpha, -color).MoveValue);
                    position.Move = childPosition;
                    UndoMove(childPosition,position, playerCurrenPosition, color);
                }
                catch (Exception e) { }
                
                if (negamaxRes > value && depth == _startDepth)
                {
                    if (position.Move is MovePlayer move)
                    {
                        position.Move = new MovePlayer(move);
                    } else position.Move = new PlaceWall((PlaceWall) position.Move);
                }
                value = Math.Max(value, negamaxRes);
                position.Move.MoveValue = value;
                //Console.WriteLine("d:" + (depth - 1) + " v:" + negamaxRes + " m:" + childPosition.Move);
                alpha = Math.Max(alpha, value);
                if (alpha >= beta) break;
            }
            return position.Move;
        }
        private LinkedList<Move> GeneratePositions(Field position, int color, int depth)
        {
            PlayerNumber currentPlayer = color == 1 ? PlayerNumber.First : PlayerNumber.Second;
            LinkedList<Move> possiblePositions = new LinkedList<Move>();
            
            foreach (var availablePositions in position.GetCellsForMove(currentPlayer))
            {
                Move newPosition = new MovePlayer(availablePositions);
                possiblePositions.AddLast(newPosition);
            }

            if (depth == _startDepth) return possiblePositions;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    WallPosition newWallPosition = new WallPosition(WallOrientation.Horizontal, new CellPosition(i, j));
                    WallPosition newWallPosition2 = new WallPosition(WallOrientation.Vertical, new CellPosition(i, j));
                    if (!position.PlacedWalls.Contains(newWallPosition))
                    {
                        possiblePositions.AddLast(new PlaceWall(newWallPosition));
                        possiblePositions.AddLast(new PlaceWall(newWallPosition2));
                    }
                }
            }
            return possiblePositions;
        }

        public void ExecuteMove(Move move, Field field, int color)
        {
            PlayerNumber player = color == 1 ? PlayerNumber.First : PlayerNumber.Second;
            if (move is MovePlayer movePlayer)
                field.MovePlayer(player, movePlayer.Position);
            else if (move is PlaceWall placeWall)
                field.PlaceWall(placeWall.Position, player);
        }
        
        public void UndoMove(Move move, Field field, CellPosition playerPosition, int color)
        {
            PlayerNumber player = color == 1 ? PlayerNumber.First : PlayerNumber.Second;
            switch (move)
            {
                case MovePlayer:
                    field.MovePlayer(player, playerPosition);
                    break;
                case PlaceWall placeWall:
                    field.RemoveWall(placeWall.Position);
                    break;
            }
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
            // if (color == 1)
            // {
            //     return (position.Player1WallAmount + (8 - player1MinLenght)) - (position.Player2WallAmount + (8 - player2MinLenght)); //need theory testing
            // } 
            // return (position.Player2WallAmount + (8 - player2MinLenght)) - (position.Player1WallAmount + (8 - player1MinLenght));
            
            // if (color == 1)
            // {
            //     return ((8 - player1MinLenght)) - ((8 - player2MinLenght)); //need theory testing
            // } 
            // return ((8 - player2MinLenght)) - ((8 - player1MinLenght));
            
            if (color == 1)
            {
                if (player2MinLenght == 0)
                { return -999;}
                if (player1MinLenght == 0)
                { return 999; }
                return (position.Player1WallAmount + (8 - player1MinLenght)) - (position.Player2WallAmount + (8 - player2MinLenght)); //need theory testing
            }
            if (player2MinLenght == 0)
            { return 999;}
            if (player1MinLenght == 0)
            { return -999; }
            return (position.Player2WallAmount + (8 - player2MinLenght)) - (position.Player1WallAmount + (8 - player1MinLenght));
        }
    }
}