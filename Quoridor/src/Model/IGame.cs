namespace Quoridor.Model
{
    public enum PlayerNumber {First, Second}
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
    public interface IGame
    {
        void StartNewGame();
        void MovePlayer(PlayerNumber playerNumber, int x, int y);
        void PlaceWall(PlayerNumber playerPlacing, WallPosition position);
    }
}