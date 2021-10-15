using System;
using Model.Api;
using Moq;

namespace Model
{
    
    public class Test
    {
        private static GameModel _gameModel;

        public static void Main(String[] args)
        { 
            AStar_pathfinding();
        }
        public static void AStar_pathfinding()
        {
            var player1Mock = Mock.Of<IPlayerView>();
            var player2Mock = Mock.Of<IPlayerView>();
            _gameModel = new GameModel(player1Mock, player2Mock);
            Console.WriteLine(_gameModel.TestIsReachable(new CellPosition(0,0), new CellPosition(2,2)));
            
            _gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallDirection.Vertical,new CellPosition(0,1), new CellPosition(1,2))); //0112
            _gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallDirection.Vertical,new CellPosition(0,3), new CellPosition(1,4))); //0314
            _gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallDirection.Horizontal,new CellPosition(1,0), new CellPosition(2,1))); //1021
            _gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallDirection.Horizontal,new CellPosition(3,0), new CellPosition(4,1))); //3041
            
            _gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallDirection.Horizontal,new CellPosition(1,4), new CellPosition(2,5))); //1425
            // _gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallDirection.Vertical,new CellPosition(4,3), new CellPosition(4,5))); //4345
            _gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallDirection.Vertical,new CellPosition(4,1), new CellPosition(5,2))); //4152
            
            _gameModel.PlaceWall(PlayerNumber.First, new WallPosition(WallDirection.Horizontal,new CellPosition(3,4), new CellPosition(4,5))); // 3445
             
            Console.WriteLine(_gameModel.TestIsReachable(new CellPosition(0,0), new CellPosition(2,2)));

            try
            {
                foreach (FieldCell cell in _gameModel.TestFindPath(new CellPosition(0,0), new CellPosition(2,2)))
                {
                    Console.WriteLine(cell.Position.X + " " + cell.Position.Y);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Null path");
            }
            
        }   
    }
}