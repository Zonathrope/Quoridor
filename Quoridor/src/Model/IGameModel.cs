namespace Quoridor.Model
{
    public enum PlayerNumber {First, Second}

    //TODO rename to IModel or smth
    public interface IGameModel
    {
        void StartNewGame();
        void MovePlayer(PlayerNumber playerNumber, int x, int y);
        void PlaceWall(PlayerNumber playerPlacing, WallPosition position);
    }
}