using Model.DataTypes;

namespace Model
{
    public record GameState(CellPosition Player1Position, CellPosition Player2Position,
        WallPosition[] PlacedWalls, bool GameEnded, PlayerNumber? Winner);
}