using Model.DataTypes;

namespace Model
{
    public interface IView
    {
        void HandleGameStartedEvent();
        void HandleGameEndedEvent();
        void HandlePlayerWonEvent(PlayerNumber winnerNumber);
        void HandlePlayerMovedEvent(PlayerNumber playerNumber, CellPosition newPosition, bool isJump = false);
        void HandlePlayerPlacedWallEvent(PlayerNumber playerPlacing, WallPosition wallPosition);
    }
}