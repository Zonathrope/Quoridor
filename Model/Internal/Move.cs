using Model.DataTypes;

namespace Model.Internal
{
    public abstract class Move
    {
    }

    public class MovePlayer : Move
    {
        public CellPosition Position { get; }
        public MovePlayer(CellPosition position)
        {
            Position = position;
        }

        public override string ToString()
        {
            return Position.X + " " + Position.Y;
        }
    }

    public class PlaceWall : Move
    {
        public WallPosition Position { get; }
        public PlaceWall(WallPosition position)
        {
            Position = position;
        }
        
        public override string ToString()
        {
            return Position.TopLeftCell.X + " " + Position.TopLeftCell.Y + " " + Position.Orientation;
        }
    }
}