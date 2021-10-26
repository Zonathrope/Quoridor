namespace Model.DataTypes
{
    public record WallPosition(WallOrientation Orientation, CellPosition TopLeftCell)
    {
        // TODO think how to rename method
        /// <summary>Determines if two wall positions exists between same four cells</summary>
        public bool IsEqualByPlace(WallPosition that)
        {
            return this.TopLeftCell == that.TopLeftCell;
        }
    };
}