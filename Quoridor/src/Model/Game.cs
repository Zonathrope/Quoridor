using System.Collections.Generic;
using System.Linq;

namespace Quoridor.Model
{
public class Game: IGame
{
    private IPlayer _player1;
    private IPlayer _player2;
    private Field _field;
    private IAStar _aStar;
    private PlayerNumber _currentPlayer;

    public Game(IPlayer player1, IPlayer player2)
    {
        _player1 = player1;
        _player2 = player2;
        StartNewGame();
    }

    public void StartNewGame()
    {
        _field = new Field();
        _currentPlayer = PlayerNumber.First;
    }

    /// <exception cref="IncorrectPlayerPositionException">Caller pass invalid position.</exception>
    /// <exception cref="CellAlreadyTakenException">Caller tries to move to taken cell.</exception>
    /// <exception cref="AnotherPlayerTurnException">Caller tries to move to taken cell.</exception>
    public void MovePlayer(PlayerNumber playerNumber, int x, int y)
    {
        if (playerNumber != _currentPlayer)
        {
            throw new AnotherPlayerTurnException($"It is player {_currentPlayer} turn");
        }
        var cellPositon = new CellPosition(x, y);
        _field.MovePlayer(playerNumber, cellPositon);
        if (IsInOpponentsEndLine(cellPositon, positionOwner: playerNumber))
        {
            HandleWin(playerNumber);
        }
        _currentPlayer = _currentPlayer == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;
    }

    /// <exception cref="IncorrectWallPositionException">Caller pass invalid position.</exception>
    /// <exception cref="WallPlaceTakenException">Caller tries to place wall over existing wall.</exception>
    /// <exception cref="WallBlocksPathForPlayerException">Caller tries to place wall that blocks way.</exception>
    public void PlaceWall(PlayerNumber playerPlacing, WallPosition position)
    {
        _field.PlaceWall(playerPlacing, position);
        if (!BothPlayersHaveWayToLastLine())
        {
            _field.RemoveWall(position);
            throw new WallBlocksPathForPlayerException(
                $"Wall between {position.TopLeftCell} and {position.BottomRightCell} blocks way for players");
        }
    }

    private bool IsInOpponentsEndLine(CellPosition position, PlayerNumber positionOwner)
    {
        if (positionOwner == PlayerNumber.First)
        {
            return position.Y == 0;
        }
        return position.Y == _field.Size;
    }

    private void HandleWin(PlayerNumber winner)
    {
        //TODO
    }

    public bool BothPlayersHaveWayToLastLine()
    {
        //TODO think if one check is enough
        List<FieldCell> player1WinLine = _field.GetPlayersWinLine(PlayerNumber.First).ToList<FieldCell>();
        FieldCell player1Cell = _field.GetPlayerCell(PlayerNumber.First);
        bool player1HasAccess = player1WinLine.Any(winCell => _aStar.WayExists(player1Cell, winCell));

        List<FieldCell> player2WinLine = _field.GetPlayersWinLine(PlayerNumber.Second).ToList<FieldCell>();
        FieldCell player2Cell = _field.GetPlayerCell(PlayerNumber.Second);
        bool player2HasAccess = player2WinLine.Any(winCell => _aStar.WayExists(player2Cell, winCell));
        return player1HasAccess && player2HasAccess;
    }
}
}