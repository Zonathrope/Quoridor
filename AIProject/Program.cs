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
            AI2 skynet2 = new AI2(6);
            
            // System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            
             // gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4,7), DrawInView.No);
             // gameModel.PlaceWall(PlayerNumber.Second, new WallPosition(WallOrientation.Vertical, new CellPosition(2,0)));
             // gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4,6), DrawInView.No);
             // gameModel.PlaceWall(PlayerNumber.Second, new WallPosition(WallOrientation.Vertical, new CellPosition(5,0)));
             // gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4,5), DrawInView.No);
             // gameModel.PlaceWall(PlayerNumber.Second, new WallPosition(WallOrientation.Horizontal, new CellPosition(4,0)));
             // gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4,4), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.Second, new CellPosition(3,0), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4,3), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.Second, new CellPosition(3,1), DrawInView.No);
             // gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallOrientation.Vertical, new CellPosition(3,0)));

             Console.WriteLine(skynet2.Negascout(gameModel.GetField(), 6,-999, +999, 1).ToString());
            // sw.Stop();
            // Console.WriteLine(       sw.Elapsed.TotalSeconds + " Sec    / " +
            //                          ((float)sw.Elapsed.TotalSeconds / (float)60).ToString("N2") + 
            //                          " min" );
            //
        }
    }
}