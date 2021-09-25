namespace Quoridor.Model
{
    public class CellPosition
    {
        public int X { set; get; }
        public int Y { set; get; }

        public CellPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static CellPosition operator +(CellPosition position1, CellPosition position2)
        {
            return new CellPosition(position1.X + position2.X, position2.Y + position2.Y);
        }

        public static bool operator ==(CellPosition position1, CellPosition position2)
        {
            return position1.X == position2.X && position1.Y == position2.Y;
        }
        public static bool operator !=(CellPosition position1, CellPosition position2)
        {
            return !(position1 == position2);
        }
    }
}