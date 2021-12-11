namespace Model.DataTypes
{
    public abstract record Move;

    public sealed record MovePlayer(PlayerNumber Player, CellPosition NewPosition) : Move;

    public sealed record Jump(PlayerNumber Player, CellPosition NewPosition) : Move;

    public sealed record PlaceWall(PlayerNumber Placer, WallPosition WallPosition) : Move;
}