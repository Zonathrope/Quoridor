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

    private Field _field;
    //TODO replace with actual implementation
    private IAStar _aStar = new AStarMock();
    private PlayerNumber _currentPlayer;

    private event Action                            GameStartedEvent;
    private event Action                            GameEndedEvent;
    private event Action<PlayerNumber>              PlayerWonEvent;
    private event Action<PlayerMovedEventArgs>      PlayerMovedEvent;
    private event Action<PlayerPlacedWallEventArgs> PlayerPlacedWallEvent;

    public int Player1WallAmount { get; private set; }
    public int Player2WallAmount { get; private set; }

    public GameModel(IPlayerView player1, IPlayerView player2)
    {
        AttachEventsToPlayer(player1);
        AttachEventsToPlayer(player2);
        _field = new Field();
    }

    private void AttachEventsToPlayer(IPlayerView player)
    {
        this.GameStartedEvent      += player.HandleGameStartedEvent;
        this.GameEndedEvent        += player.HandleGameEndedEvent;
        this.PlayerWonEvent        += player.HandlePlayerWonEvent;
        this.PlayerMovedEvent      += player.HandlePlayerMovedEvent;
        this.PlayerPlacedWallEvent += player.HandlePlayerPlacedWallEvent;
    }

    public void StartNewGame()
    {
        _field = new Field();
        _currentPlayer = PlayerNumber.First;
        Player1WallAmount = GameConstants.StartWallAmount;
        Player2WallAmount = GameConstants.StartWallAmount;
        GameStartedEvent?.Invoke();
    }

    public void EndGame()
    {
        GameEndedEvent?.Invoke();
    }

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
        PlayerMovedEvent?.Invoke(new PlayerMovedEventArgs(playerNumber, newPosition));
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
        CellPosition neighborCellTakenByOpponent = reachableCells.Find(
            position => _field.IsCellTaken(position));
        if (neighborCellTakenByOpponent is null)
            return reachableCells;
        CellPosition opponentPosition = neighborCellTakenByOpponent;
        reachableCells.Remove(opponentPosition);
        reachableCells.AddRange(
            CellsAvailableFromFaceToFaceSituation(playerCurrentPosition, opponentPosition));
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
            List<CellPosition> opponentNeighbours = _field.GetReachableNeighbours(opponentPosition);
            opponentNeighbours.Remove(playerCurrentPosition);
            opponentNeighbours.Remove(cellBehindOpponent);
            availableCells.AddRange(opponentNeighbours);
        }
        return availableCells;
    }

    public void PlaceWall(PlayerNumber playerPlacing, WallPosition wallPosition)
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
        _field.PlaceWall(wallPosition);
        if (!BothPlayersHaveWayToLastLine())
        {
            _field.RemoveWall(wallPosition);
            var wallDirection = wallPosition.Direction == WallDirection.Horizontal ? "Horizontal" : "Vertical";
            throw new WallBlocksPathForPlayerException(
                $"{wallDirection} wall at {wallPosition.TopLeftCell} blocks way for players");
        }
        SwitchCurrentPlayer();
        PlayerPlacedWallEvent?.Invoke(new PlayerPlacedWallEventArgs(playerPlacing, wallPosition));
    }

    private bool DoesPlayerHasWalls(PlayerNumber playerNumber)
    {
        int wallAmount = playerNumber == PlayerNumber.First ? Player1WallAmount : Player2WallAmount;
        return wallAmount != 0;
    }

    private void DecrementPlayerWallAmount(PlayerNumber playerNumber)
    {
        if (playerNumber == PlayerNumber.First)
            Player1WallAmount--;
        else
            Player2WallAmount--;
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
        PlayerWonEvent?.Invoke(winner);
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