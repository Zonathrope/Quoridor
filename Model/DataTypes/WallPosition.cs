namespace Model.DataTypes
{
    public record WallPosition(
        WallOrientation Orientation,
        CellPosition TopLeftCell);
}