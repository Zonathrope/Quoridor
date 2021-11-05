using System;
using Model;
using Model.DataTypes;
using Model.Internal;

namespace AIProject
{
    class Program
    {
        static void Main(string[] args)
        {
            IGameModel gameModel = new GameModel();
            
            Ai skynet = new Ai(5);
            AStar astar = new AStar();
            Console.WriteLine(skynet.Negamax(gameModel.GetField(), 5,-999, +999, 1).ToString());
            //gameModel.GetField().MovePlayer(PlayerNumber.First, new CellPosition(3,8));
            
            //gameModel.GetField().MovePlayer(PlayerNumber.First, new CellPosition(4,7));
            //gameModel.GetField().MovePlayer(PlayerNumber.Second, new CellPosition(4,1));
            //Console.WriteLine(skynet.Sev(gameModel.GetField(), 1));
            
            Console.ReadLine();
        }
    }
}