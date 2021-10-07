using System.Collections.Generic;

namespace Model.Api
{
    public interface IGameModel
    {
        void StartNewGame();
        void EndGame();
        // TODO think to change call signature to CellPosition instead of x and y
        void MovePlayer(PlayerNumber playerNumber, int x, int y);
        void PlaceWall(PlayerNumber playerPlacing, WallPosition position);
        List<CellPosition> GetCellsAvailableForMove(PlayerNumber playerNumber);
        CellPosition Player1Position{ get; }
        CellPosition Player2Position{ get; }
        CellPosition GetPlayerPosition(PlayerNumber playerNumber);
        public List<FieldCell> TestFindPath(CellPosition startPos, CellPosition endPos);
        public bool TestIsReachable(CellPosition startPos, CellPosition endPos);
    }
}