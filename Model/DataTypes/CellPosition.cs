using System;

namespace Model.DataTypes
{
    public record CellPosition
    {
        public int X { get; }
        public int Y { get; }

        /// <exception cref="ArgumentOutOfRangeException">X or Y is below 0</exception>
        public CellPosition(int x, int y)
        {
            if (x < 0 || y < 0)
                throw new ArgumentOutOfRangeException($"both x and y must be > 0, got x={x}, y={y}");
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"[{X},{Y}]";
        }

        /// <exception cref="ArgumentOutOfRangeException">Shift result in cell with negative coordinates</exception>
        public CellPosition Shifted(int shiftX, int shiftY)
        {
            return new CellPosition(this.X + shiftX, this.Y + shiftY);
        }
    }
}