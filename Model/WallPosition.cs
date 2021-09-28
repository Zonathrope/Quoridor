namespace Model
{
    public enum WallDirection {Vertical, Horizontal}
    public class WallPosition
    {
        public WallDirection Direction { get; }
        public CellPosition TopLeftCell { get; }
        public CellPosition BottomRightCell { get; }
        WallPosition(WallDirection direction, CellPosition topLeftCell, CellPosition bottomRightCell)
        {
            Direction = direction;
            TopLeftCell = topLeftCell;
            BottomRightCell = bottomRightCell;
        }
    }
}