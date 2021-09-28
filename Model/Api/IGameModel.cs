using System.Collections.Generic;

namespace Model.Api
{
    public interface IGameModel
    {
        void StartNewGame();
        void EndGame();
        void MovePlayer(PlayerNumber playerNumber, int x, int y);
        void PlaceWall(PlayerNumber playerPlacing, WallPosition position);
        List<CellPosition> GetCellsAvailableForMove(PlayerNumber playerNumber);
    }
}