using Model.DataTypes;

namespace Model
{
    public static class GameConstants
    {
        public const int FieldSize = 9;
        // TODO look if coordinate can be useful elsewhere
        public const int FieldEndCoordinate = FieldSize - 1;
        private const int FieldMiddleCoordinate = 4;
        public static readonly CellPosition Player1StartPosition = new (FieldMiddleCoordinate, FieldEndCoordinate);
        public static readonly CellPosition Player2StartPosition = new (FieldMiddleCoordinate, 0);
        public const int StartWallAmount = 10;

        public static readonly CellPosition[] Player1WinLine =
        {
            new CellPosition(0, 0),
            new CellPosition(1, 0),
            new CellPosition(2, 0),
            new CellPosition(3, 0),
            new CellPosition(4, 0),
            new CellPosition(5, 0),
            new CellPosition(6, 0),
            new CellPosition(7, 0),
            new CellPosition(8, 0)
        };

        public static readonly CellPosition[] Player2WinLine =
        {
            new CellPosition(0, FieldEndCoordinate),
            new CellPosition(1, FieldEndCoordinate),
            new CellPosition(2, FieldEndCoordinate),
            new CellPosition(3, FieldEndCoordinate),
            new CellPosition(4, FieldEndCoordinate),
            new CellPosition(5, FieldEndCoordinate),
            new CellPosition(6, FieldEndCoordinate),
            new CellPosition(7, FieldEndCoordinate),
            new CellPosition(8, FieldEndCoordinate)
        };
    }
}