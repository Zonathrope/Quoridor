using System;
using System.Collections.Generic;
using Model.DataTypes;

namespace AI
{
    public class AI
    {
        public int Negamax(Field position, int depth, int alpha, int beta, int color)
        {
            if (depth == 0 || CheckWin(position, color))
            {
                return Sev(position) * color;
            }
            var childPositions = generatePositions(position);
            childPositions = orderPositions(childPositions);
            var value = -9999;
            foreach (Field childPosition in childPositions)
            {
                value = Math.Max(value, -Negamax(childPosition, depth - 1, -alpha, -beta, -color));
                alpha = Math.Max(alpha, value);
                if (alpha >= beta) break;
            }
            return value;
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
            throw new System.NotImplementedException();
        }

        int Sev(Field position){
            return 1;
        }
    }
    
    
}