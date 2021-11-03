using System;
using System.Collections.Generic;
using Model;
using Model.DataTypes;
using Model.Internal;

namespace AI
{
    public class AI
    {
        private AStar _aStar = new AStar();
        public Field Negamax(Field position, int depth, int alpha, int beta, int color)
        {
            if (depth == 0 || CheckWin(position, color))
            {
                position.positionValue = Sev(position) * color;
                return position;
            }
            var childPositions = generatePositions(position);
            childPositions = orderPositions(childPositions);
            var value = -999;
            Field bestPosition = null;
            foreach (Field childPosition in childPositions)
            {
                value = Math.Max(value, -Negamax(childPosition, depth - 1, -alpha, -beta, -color).positionValue);
                position.positionValue = value;
                if (bestPosition == null || bestPosition.positionValue < value) {
                    bestPosition = position;
                }
                alpha = Math.Max(alpha, value);
                if (alpha >= beta) break;
            }
            return bestPosition;
        }

        private LinkedList<Field> orderPositions(object childPositions)
        {
            throw new System.NotImplementedException();
        }

        private LinkedList<Field> generatePositions(Field position)
        {
            throw new System.NotImplementedException();
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

        int Sev(Field position)
        {
            int player1MinLenght = 0;
            int player2MinLenght = 0;
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
            return 1;
        }
    }
    
    
}