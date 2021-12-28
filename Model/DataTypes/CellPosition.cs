namespace Model.DataTypes
{
    public struct CellPosition
    {
        public CellPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(CellPosition position)
        {
            return this.X == position.X && this.Y == position.Y;
        }
        public int X { get; }
        public int Y { get; }

        public override string ToString() => $"({X}, {Y})";

        public CellPosition Shifted(int shiftX, int shiftY)
        {
            return new CellPosition(this.X + shiftX, this.Y + shiftY);
        }
    }
}