using System;
using System.Collections.Generic;
using System.Linq;
using Model.DataTypes;
using Model.Internal;

namespace Model
{
public class GameModel: IGameModel
{
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
    private IAStar _aStar = new AStarMock();
    private PlayerNumber _currentPlayer;

    private event EventHandler RaiseGameStartedEvent;
    private event EventHandler RaiseGameEndedEvent;
    private event EventHandler<PlayerWonEventArgs> RaisePlayerWonEvent;
    private event EventHandler<PlayerMovedEventArgs> RaisePlayerMovedEvent;
    private event EventHandler<PlayerPlacedWallEventArgs> RaisePlayerPlacedWallEvent;

    private const int StartWallAmount = 10;
    public int Player1WallAmount => _player1WallAmount;
    public int Player2WallAmount => _player1WallAmount;
    private int _player1WallAmount;
    private int _player2WallAmount;
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
        _player1WallAmount = StartWallAmount;
        _player2WallAmount = StartWallAmount;
        RaiseGameStartedEvent?.Invoke(this, EventArgs.Empty);
    }

    public void EndGame()
    {
        RaiseGameEndedEvent?.Invoke(this, EventArgs.Empty);
    }

    /// <exception cref="IncorrectPlayerPositionException">Caller pass invalid position.</exception>
    /// <exception cref="CellAlreadyTakenException">Caller tries to move to taken cell.</exception>
    /// <exception cref="AnotherPlayerTurnException">Caller tries to move to taken cell.</exception>
    public void MovePlayer(PlayerNumber playerNumber, CellPosition newPosition)
    {
        if (!IsThisPlayersTurn(playerNumber))
        {
            throw new AnotherPlayerTurnException($"It is player {_currentPlayer} turn");
        }

        CellPosition oldPosition = _field.GetPlayerPosition(playerNumber);

        List<CellPosition> neighbours = _field.GetReachableNeighbours(oldPosition);
        if (!GetCellsAvailableForMove(playerNumber).Contains(newPosition))
        {
            throw new IncorrectPlayerPositionException(
                $"Can't move from {_field.GetPlayerPosition(playerNumber)} to {newPosition}");
        }
        _field.MovePlayer(playerNumber, newPosition);
        SwitchCurrentPlayer();
        RaisePlayerMovedEvent?.Invoke(this, new PlayerMovedEventArgs(playerNumber, newPosition));
        if (IsInOpponentsEndLine(newPosition, positionOwner: playerNumber))
        {
            HandleWin(playerNumber);
        }

    }

    private void SwitchCurrentPlayer()
    {
        _currentPlayer = _currentPlayer == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;
    }

    public List<CellPosition> GetCellsAvailableForMove(PlayerNumber playerNumber)
    {
        CellPosition playerCurrentPosition = _field.GetPlayerPosition(playerNumber);
        List<CellPosition> reachableCells = _field.GetReachableNeighbours(playerCurrentPosition);
        CellPosition neighborCellTakenByOpponent = null;
        //TODO think about replacing cycle
        foreach (CellPosition position in reachableCells)
        {
            if (_field.IsCellTaken(position))
            {
                neighborCellTakenByOpponent = position;
                break;
            }
        }
        if (!(neighborCellTakenByOpponent is null))
        {
            CellPosition opponentPosition = neighborCellTakenByOpponent;
            reachableCells.Remove(opponentPosition);
            reachableCells.AddRange(
                CellsAvailableFromFaceToFaceSituation(playerCurrentPosition, opponentPosition));
        }
        return reachableCells;
    }

    /// <summary>
    /// For the Quoridor game the situation where opponents face each over face to face is
    /// threatened using some special rules.
    /// </summary>
    /// <returns>Cells available for player to move due face to face situation.
    /// Don't include cells available by regular rules</returns>
    private List<CellPosition> CellsAvailableFromFaceToFaceSituation(
        CellPosition playerCurrentPosition,
        CellPosition opponentPosition)
    {
        List<CellPosition> availableCells = new List<CellPosition>();
        int positionDifferenceX = opponentPosition.X - playerCurrentPosition.X;
        int positionDifferenceY = opponentPosition.Y - playerCurrentPosition.Y;
        // Cell behind opponent is acquired by finding next cell from player position in opponents
        // direction
        CellPosition cellBehindOpponent = opponentPosition.Shifted(positionDifferenceX, positionDifferenceY);
        if (_field.WayBetweenCellsExists(opponentPosition, cellBehindOpponent))
        {
            availableCells.Add(cellBehindOpponent);
        }
        else
        {
            //TODO think about is treating  blocked neighbours as not neighbours is ok
            List<CellPosition> opponentNeighbours = _field.GetReachableNeighbours(opponentPosition);
            opponentNeighbours.Remove(playerCurrentPosition);
            opponentNeighbours.Remove(cellBehindOpponent);
            availableCells.AddRange(opponentNeighbours);
        }
        return availableCells;
    }

    /// <exception cref="NoWallsLeftException">Player has no walls.</exception>
    /// <exception cref="IncorrectWallPositionException">Caller pass invalid position.</exception>
    /// <exception cref="WallPlaceTakenException">Caller tries to place wall over existing wall.</exception>
    /// <exception cref="WallBlocksPathForPlayerException">Caller tries to place wall that blocks way.</exception>
    public void PlaceWall(PlayerNumber playerPlacing, WallPosition position)
    {
        if (!DoesPlayerHasWalls(playerPlacing))
        {
            throw new NoWallsLeftException($"Player {playerPlacing} has no walls left");
        }
        DecrementPlayerWallAmount(playerPlacing);
        if (!IsThisPlayersTurn(playerPlacing))
        {
            throw new AnotherPlayerTurnException($"It is player {_currentPlayer} turn");
        }
        _field.PlaceWall(position);
        if (!BothPlayersHaveWayToLastLine())
        {
            _field.RemoveWall(position);
            throw new WallBlocksPathForPlayerException(
                $"Wall between {position.TopLeftCell} and {position.BottomRightCell} blocks way for players");
        }
        SwitchCurrentPlayer();
        RaisePlayerPlacedWallEvent?.Invoke(this, new PlayerPlacedWallEventArgs(playerPlacing, position));
    }

    private bool DoesPlayerHasWalls(PlayerNumber playerNumber)
    {
        int wallAmount = playerNumber == PlayerNumber.First ? _player1WallAmount : _player2WallAmount;
        return wallAmount != 0;
    }

    private void DecrementPlayerWallAmount(PlayerNumber playerNumber)
    {
        if (playerNumber == PlayerNumber.First)
            _player1WallAmount--;
        else
            _player2WallAmount--;
    }

    private bool IsThisPlayersTurn(PlayerNumber playerNumber)
    {
        return playerNumber == _currentPlayer;
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