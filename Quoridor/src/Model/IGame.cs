namespace Quoridor.Model
{
    public enum PlayerNumber {First, Second}

    public interface IGame
    {
        void StartNewGame();
        void MovePlayer(PlayerNumber playerNumber, int x, int y);
        void PlaceWall(PlayerNumber playerPlacing, WallPosition position);
    }
}