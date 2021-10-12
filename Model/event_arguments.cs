using Model.DataTypes;

namespace Model
{
    public record PlayerMovedEventArgs(PlayerNumber PlayerNumber, CellPosition Position);
    public record PlayerPlacedWallEventArgs(PlayerNumber PlayerNumber, WallPosition WallPosition);
}