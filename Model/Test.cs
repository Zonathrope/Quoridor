using System;
using Model.DataTypes;
using Model.Internal;
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
            Field field = new Field();
            AStar aStar = new AStar();

            Console.WriteLine(aStar.WayExists(new CellPosition(0,0), new CellPosition(2,2), field));
            field.PlaceWall(new WallPosition(WallOrientation.Vertical, new CellPosition(0, 1))); //0112
            field.PlaceWall(new WallPosition(WallOrientation.Vertical, new CellPosition(0, 3))); //0314
            field.PlaceWall(new WallPosition(WallOrientation.Horizontal, new CellPosition(1, 0))); //1021
            field.PlaceWall(new WallPosition(WallOrientation.Horizontal, new CellPosition(3, 0))); //3041
            field.PlaceWall(new WallPosition(WallOrientation.Horizontal, new CellPosition(1, 4))); //1425
            field.PlaceWall(new WallPosition(WallOrientation.Vertical, new CellPosition(4, 3))); //4354
            field.PlaceWall(new WallPosition(WallOrientation.Vertical, new CellPosition(4, 1))); //4152
            field.PlaceWall(new WallPosition(WallOrientation.Horizontal,new CellPosition(3,4))); // 3445
            Console.WriteLine(aStar.WayExists(new CellPosition(0,0), new CellPosition(2,2), field));

            try
            {
                 foreach (FieldCell cell in aStar.FindPath(field.FieldMatrix[0,0], field.FieldMatrix[2,2]))
                 {
                     Console.WriteLine(cell.Position.X + " " + cell.Position.Y);
                 }
            }
            catch (Exception e)
            { Console.WriteLine("Null path");}
        }   
    }
}