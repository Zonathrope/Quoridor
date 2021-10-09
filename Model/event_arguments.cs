using System;
using Model.DataTypes;

namespace Model
{
    public record PlayerMovedEventArgs(PlayerNumber PlayerNumber, CellPosition Position);
    //TODO rename position
    public record PlayerPlacedWallEventArgs(PlayerNumber PlayerNumber, WallPosition Position);
}