namespace Quoridor.Model
{
    public class CellPosition
    {
        public int X { get; }
        public int Y { get; }

        public CellPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static CellPosition operator +(CellPosition position1, CellPosition position2)
        {
            return new CellPosition(position1.X + position2.X, position2.Y + position2.Y);
        }

        public static CellPosition operator -(CellPosition position1, CellPosition position2)
        {
            return new CellPosition(position2.X - position1.X, position2.Y - position1.Y);
        }

        public static bool operator ==(CellPosition position1, CellPosition position2)
        {
            if (position1 is null) return false;
            if (position2 is null) return false;
            return position1.X == position2.X && position1.Y == position2.Y;
        }
        public static bool operator !=(CellPosition position1, CellPosition position2)
        {
            return !(position1 == position2);
        }
        protected bool Equals(CellPosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.
                GetType() != this.GetType()) return false;
            return Equals((CellPosition) obj);
        }

        public override string ToString()
        {
            return $"[{X},{Y}]";
        }
    }
}