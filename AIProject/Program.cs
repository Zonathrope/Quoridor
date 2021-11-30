using System;
using Model;
using Model.DataTypes;

namespace AIProject
{
    static class Program
    {
        static void Main(string[] args)
        {
            IGameModel gameModel = new GameModel();
            AI2 skynet2 = new AI2(2);
            
            // System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            
            gameModel.GetField().Player1Position = new CellPosition(8, 1);
            gameModel.GetField().Player2Position = new CellPosition(4, 5);
            
             gameModel.PlaceWall(PlayerNumber.First, 
                 new WallPosition(WallOrientation.Vertical, new CellPosition(2,7)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.Second, 
                 new WallPosition(WallOrientation.Vertical, new CellPosition(5,7)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.First, 
                 new WallPosition(WallOrientation.Horizontal, new CellPosition(4,5)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.Second, 
                 new WallPosition(WallOrientation.Horizontal, new CellPosition(2,5)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.First, 
                 new WallPosition(WallOrientation.Vertical, new CellPosition(1,6)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.Second, 
                 new WallPosition(WallOrientation.Horizontal, new CellPosition(2,6)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.First, 
                 new WallPosition(WallOrientation.Vertical, new CellPosition(4,6)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.Second, 
                 new WallPosition(WallOrientation.Horizontal, new CellPosition(5,6)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.First, 
                 new WallPosition(WallOrientation.Horizontal, new CellPosition(3,4)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.Second, 
                 new WallPosition(WallOrientation.Horizontal, new CellPosition(6,5)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.First, 
                 new WallPosition(WallOrientation.Vertical, new CellPosition(7,6)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.Second, 
                 new WallPosition(WallOrientation.Horizontal, new CellPosition(6,7)), DrawInView.No);
             gameModel.PlaceWall(PlayerNumber.First, 
                 new WallPosition(WallOrientation.Horizontal, new CellPosition(7,3)), DrawInView.No);

             Console.WriteLine(skynet2.Negascout(gameModel.GetField(), 2,-999, +999, -1).ToString());
             //Console.WriteLine(skynet2.GetMove(gameModel.GetField(), PlayerNumber.Second, false).ToString());
             // sw.Stop();
             // Console.WriteLine(       sw.Elapsed.TotalSeconds + " Sec    / " +
             //                          ((float)sw.Elapsed.TotalSeconds / (float)60).ToString("N2") + 
             //                          " min" );
             //
        }
    }
}