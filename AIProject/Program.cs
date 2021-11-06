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
            Ai skynet = new Ai(7);
            AStar astar = new AStar();
            
            //gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallOrientation.Horizontal, new CellPosition(1,0)));
            //gameModel.PlaceWall(PlayerNumber.Second, new WallPosition(WallOrientation.Vertical, new CellPosition(0,1)));
            //gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallOrientation.Horizontal, new CellPosition(1,2)));
            //gameModel.PlaceWall(PlayerNumber.Second, new WallPosition(WallOrientation.Vertical, new CellPosition(2,1)));
            
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            
            //var count = astar.FindPath(new CellPosition(8,8), new CellPosition(1,1), gameModel.GetField()).Count;
            
            Console.WriteLine(skynet.Negamax(gameModel.GetField(), 7,-999, +999, 1).ToString());
            
            //astar.WayExists(new CellPosition(8, 8), new CellPosition(1, 1), gameModel.GetField());
            sw.Stop();
            Console.WriteLine(       sw.Elapsed.TotalSeconds + " Sec    / " +
                                 ((float)sw.Elapsed.TotalSeconds / (float)60).ToString("N2") + 
                                 " min" );
            //Console.WriteLine(count);
            //Environment.Exit(0);
        }
    }
}