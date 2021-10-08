namespace Model.DataTypes
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
        public bool Equals(CellPosition other)
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

        public CellPosition Shifted(int shiftX, int shiftY)
        {
            return new CellPosition(this.X + shiftX, this.Y + shiftY);
        }
    }
}