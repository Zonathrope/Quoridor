using Model.Api;

namespace Model.DataTypes
{
    public record WallPosition(
        WallDirection Direction,
        CellPosition TopLeftCell, CellPosition BottomRightCell);
}