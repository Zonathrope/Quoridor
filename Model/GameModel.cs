using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Model;
using Model.Api;
using Moq;

namespace Model
{

public class GameModel: IGameModel
{
    //TODO add check for moving to same position
    public CellPosition Player1Position => _field.Player1Position;
    public CellPosition Player2Position => _field.Player2Position;
    public CellPosition GetPlayerPosition(PlayerNumber playerNumber)
    {
        return playerNumber == PlayerNumber.First ? Player1Position : Player2Position;
    }

    public List<WallPosition> PlacedWalls => _field.PlacedWalls;
    //TODO maybe somehow expose constants in one class
    public int FieldSize => _field.Size;
    public int FieldMiddle => _field.FieldMiddleCoordinate;

    private IPlayerView _player1;
    private IPlayerView _player2;
    private Field _field;
    //TODO replace with actual implementation
    private IAStar _aStar = Mock.Of<IAStar>();
    private PlayerNumber _currentPlayer;

    private event EventHandler RaiseGameStartedEvent;
    private event EventHandler RaiseGameEndedEvent;
    private event EventHandler<PlayerWonEventArgs> RaisePlayerWonEvent;
    private event EventHandler<PlayerMovedEventArgs> RaisePlayerMovedEvent;
    private event EventHandler<PlayerPlacedWallEventArgs> RaisePlayerPlacedWallEvent;

    public GameModel(IPlayerView player1, IPlayerView player2)
    {
        _player1 = player1;
        _player2 = player2;
        AttachEventsToPlayer(_player1);
        AttachEventsToPlayer(_player2);
        _field = new Field();
    }

    private void AttachEventsToPlayer(IPlayerView player)
    {
        this.RaiseGameStartedEvent      += player.HandleGameStartedEvent;
        this.RaiseGameEndedEvent        += player.HandleGameEndedEvent;
        this.RaisePlayerWonEvent        += player.HandlePlayerWonEvent;
        this.RaisePlayerMovedEvent      += player.HandlePlayerMovedEvent;
        this.RaisePlayerPlacedWallEvent += player.HandlePlayerPlacedWallEvent;
    }

    public void StartNewGame()
    {
        _field = new Field();
        _currentPlayer = PlayerNumber.First;
        RaiseGameStartedEvent?.Invoke(this, EventArgs.Empty);
    }

    public void EndGame()
    {
        RaiseGameEndedEvent?.Invoke(this, EventArgs.Empty);
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

        CellPosition oldPosition = _field.GetPlayerPosition(playerNumber);
        var newPosition = new CellPosition(x, y);

        List<CellPosition> neighbours = _field.GetNeighboursPositions(oldPosition);
        if (!GetCellsAvailableForMove(playerNumber).Contains(newPosition))
        {
            throw new IncorrectPlayerPositionException(
                $"Can't move from {_field.GetPlayerPosition(playerNumber)} to {newPosition}");
        }
        _field.MovePlayer(playerNumber, newPosition);
        _currentPlayer = _currentPlayer == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;
        RaisePlayerMovedEvent?.Invoke(this, new PlayerMovedEventArgs(playerNumber, newPosition));
        if (IsInOpponentsEndLine(newPosition, positionOwner: playerNumber))
        {
            HandleWin(playerNumber);
        }
    }

    public List<CellPosition> GetCellsAvailableForMove(PlayerNumber playerNumber)
    {
        CellPosition currentPosition = _field.GetPlayerPosition(playerNumber);
        List<CellPosition> result = _field.GetNeighboursPositions(currentPosition);
        CellPosition neighborCellTakenByOpponent = null;
        foreach (CellPosition position in result)
        {
            if (_field.IsCellTaken(position))
            {
                neighborCellTakenByOpponent = position;
                break;
            }
        }

        if (neighborCellTakenByOpponent is null)
        {
            return result;
        }
        result.Remove(neighborCellTakenByOpponent);
        //TODO think if storing offset in same object is adequate
        //maybe i should introduce celloffset object
        CellPosition opponentPosition = neighborCellTakenByOpponent;
        CellPosition moveOffset = opponentPosition - currentPosition;
        CellPosition cellBehindOpponent = opponentPosition + moveOffset;
        if (_field.WayBetweenCellsExists(opponentPosition, cellBehindOpponent))
        {
            result.Add(cellBehindOpponent);
        }
        else
        {
            //TODO think about is treating  blocked neighbours as not neighbours is ok
            List<CellPosition> opponentNeighbours = _field.GetNeighboursPositions(opponentPosition);
            opponentNeighbours.Remove(currentPosition);
            opponentNeighbours.Remove(cellBehindOpponent);
            result.AddRange(opponentNeighbours);
        }
        return result;
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
        RaisePlayerPlacedWallEvent?.Invoke(this, new PlayerPlacedWallEventArgs(playerPlacing, position));
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
        RaisePlayerWonEvent?.Invoke(this, new PlayerWonEventArgs(winner));
    }

    private bool BothPlayersHaveWayToLastLine()
    {
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