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
            //gameModel.GetField().ExecuteMove(new PlaceWall(new WallPosition(WallOrientation.Vertical, new CellPosition(2,7))), 1);
            //gameModel.GetField().UndoMove(new PlaceWall(new WallPosition(WallOrientation.Vertical, new CellPosition(2,7))), new CellPosition(4,8), 1);
            
            // System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            
             // gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallOrientation.Vertical, new CellPosition(2,7)), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.Second, new CellPosition(4,1), DrawInView.No);
             // gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallOrientation.Vertical, new CellPosition(5,7)), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.Second, new CellPosition(4,2), DrawInView.No);
             // gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallOrientation.Horizontal, new CellPosition(4,5)), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.Second, new CellPosition(3,2), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4,7), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.Second, new CellPosition(3,3), DrawInView.No);
             // gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallOrientation.Vertical, new CellPosition(3,7)), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4,4), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.Second, new CellPosition(3,0), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4,3), DrawInView.No);
             // gameModel.MovePlayer(PlayerNumber.Second, new CellPosition(3,1), DrawInView.No);
             // gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallOrientation.Vertical, new CellPosition(3,0)));

             Console.WriteLine(skynet2.Negascout(gameModel.GetField(), 2,-999, +999, 1).ToString());
            // sw.Stop();
            // Console.WriteLine(       sw.Elapsed.TotalSeconds + " Sec    / " +
            //                          ((float)sw.Elapsed.TotalSeconds / (float)60).ToString("N2") + 
            //                          " min" );
            //
        }
    }
}