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

    public void MovePlayer(PlayerNumber playerNumber, int x, int y)
    {
        throw new System.NotImplementedException();
    }

    public void PlaceWall(PlayerNumber playerPlacing, WallPosition position)
    {
        throw new System.NotImplementedException();
    }
}
}