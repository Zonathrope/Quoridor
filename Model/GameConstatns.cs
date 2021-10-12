using Model.DataTypes;

namespace Model
{
    public static class GameConstants
    {
        public const int FieldSize = 9;
        private const int FieldMiddleCoordinate = 4;
        public static readonly CellPosition Player1DefaultPosition = new (FieldMiddleCoordinate, FieldSize - 1);
        public static readonly CellPosition Player2DefaultPosition = new (FieldMiddleCoordinate, 0);
        public const int StartWallAmount = 10;
    }
}