using System;

namespace Model.DataTypes
{
    public record CellPosition
    {
        public int X { get; }
        public int Y { get; }

        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public CellPosition(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                throw new ArgumentOutOfRangeException($"both x and y must be > 0, got x={x}, y={y}");
            }
            X = x;
            Y = y;
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