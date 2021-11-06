using Model.DataTypes;

namespace Model
{
    public abstract class Move
    {
        public int MoveValue;
        
    }

    public class MovePlayer : Move
    {
        public CellPosition Position { get; }
        public MovePlayer(CellPosition position)
        {
            Position = position;
        }

        public MovePlayer(MovePlayer other)
        {
            this.Position = new CellPosition(other.Position.X, other.Position.Y);
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

        public PlaceWall(PlaceWall other)
        {
            this.Position = new WallPosition(other.Position.Orientation,
                new CellPosition(other.Position.TopLeftCell.X, other.Position.TopLeftCell.Y));
        }
        public override string ToString()
        {
            return Position.TopLeftCell.X + " " + Position.TopLeftCell.Y + " " + Position.Orientation;
        }
    }
}