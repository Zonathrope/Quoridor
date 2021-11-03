using Model.DataTypes;

namespace Model
{
    public static class GameConstants
    {
        public const int FieldSize = 9;
        public static readonly CellPosition Player1StartPosition = new (4, 8);
        public static readonly CellPosition Player2StartPosition = new (4, 0);
        public const int StartWallAmount = 10;

        public static readonly CellPosition[] Player1WinLine =
        {
            new (0, 0),
            new (1, 0),
            new (2, 0),
            new (3, 0),
            new (4, 0),
            new (5, 0),
            new (6, 0),
            new (7, 0),
            new (8, 0)
        };

        public static readonly CellPosition[] Player2WinLine =
        {
            new (0, 8),
            new (1, 8),
            new (2, 8),
            new (3, 8),
            new (4, 8),
            new (5, 8),
            new (6, 8),
            new (7, 8),
            new (8, 8)
        };
    }
}