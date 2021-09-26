namespace Quoridor.Model
{
public class Game: IGame
{
    private IPlayer _player1;
    private IPlayer _player2;
    private Field _field;
    public Game(IPlayer player1, IPlayer player2)
    {
        _player1 = player1;
        _player2 = player2;
    }

    public void StartNewGame()
    {
        _field = new Field();
    }

    /// <exception cref="IncorrectPlayerPositionException">Caller pass invalid position.</exception>
    /// <exception cref="CellAlreadyTakenException">Caller tries to move to taken cell.</exception>
    public void MovePlayer(PlayerNumber playerNumber, int x, int y)
    {
        throw new System.NotImplementedException();
    }

    /// <exception cref="IncorrectWallPositionException">Caller pass invalid position.</exception>
    /// <exception cref="WallPlaceTakenException">Caller tries to place wall over existing wall.</exception>
    public void PlaceWall(PlayerNumber playerPlacing, WallPosition position)
    {
        throw new System.NotImplementedException();
    }
}
}