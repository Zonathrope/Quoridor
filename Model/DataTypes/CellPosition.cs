namespace Model.DataTypes
{
    public record CellPosition(int X, int Y)
    {
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