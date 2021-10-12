﻿using System;
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
    public int Player1WallAmount { get; private set; }
    public int Player2WallAmount { get; private set; }
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

        if (!GetCellsAvailableForMove(playerNumber).Contains(newPosition))
        {
            throw new IncorrectPlayerPositionException(
                $"Can't move from {_field.GetPlayerPosition(playerNumber)} to {newPosition}");
        }
        _field.MovePlayer(playerNumber, newPosition);
        SwitchCurrentPlayer();
        PlayerMovedEvent?.Invoke(new PlayerMovedEventArgs(playerNumber, newPosition));
        if (IsOnWinningPosition(playerNumber))
        {
            HandleWin(playerNumber);
        }
    }

    private void SwitchCurrentPlayer()
    {
        _currentPlayer = GetOppositePlayerNumber(_currentPlayer);
    }

    private static PlayerNumber GetOppositePlayerNumber(PlayerNumber playerNumber)
    {
        return playerNumber == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;
    }

    public List<CellPosition> GetCellsAvailableForMove(PlayerNumber playerNumber)
    {
        CellPosition playerCurrentPosition = _field.GetPlayerPosition(playerNumber);
        CellPosition opponentPosition = GetPlayerPosition(GetOppositePlayerNumber(playerNumber));
        List<CellPosition> reachableCells = _field.GetReachableNeighbours(playerCurrentPosition);
        if (reachableCells.Contains(opponentPosition))
        {
            reachableCells.Remove(opponentPosition);
            reachableCells.AddRange(
                GetCellsAvailableFromFaceToFaceSituation(playerCurrentPosition, opponentPosition));
        }
        return reachableCells;
    }

    /// <summary>
    /// For the Quoridor game the situation when opponents face each over face to face is
    /// threatened using some special rules.
    /// </summary>
    /// <returns>Cells available for player to move due face to face situation.
    /// Don't include cells available by regular rules</returns>
    private List<CellPosition> GetCellsAvailableFromFaceToFaceSituation(
        CellPosition playerPosition, CellPosition opponentPosition)
    {
        var availableCells = new List<CellPosition>();
        int positionDifferenceX = opponentPosition.X - playerPosition.X;
        int positionDifferenceY = opponentPosition.Y - playerPosition.Y;
        // Cell behind opponent is acquired by finding next cell from player position in opponents direction
        CellPosition cellBehindOpponent = opponentPosition.Shifted(positionDifferenceX, positionDifferenceY);
        if (_field.WayBetweenExists(opponentPosition, cellBehindOpponent))
        {
            availableCells.Add(cellBehindOpponent);
        }
        else
        {
            List<CellPosition> opponentNeighbours = _field.GetReachableNeighbours(opponentPosition);
            opponentNeighbours.Remove(playerPosition);
            opponentNeighbours.Remove(cellBehindOpponent);
            availableCells.AddRange(opponentNeighbours);
        }
        return availableCells;
    }

    public void PlaceWall(PlayerNumber playerPlacing, WallPosition wallPosition)
    {
        if (!IsThisPlayersTurn(playerPlacing))
        {
            throw new AnotherPlayerTurnException($"It is player {_currentPlayer} turn");
        }
        if (!PlayerHasWalls(playerPlacing))
        {
            throw new NoWallsLeftException($"Player {playerPlacing} has no walls left");
        }
        _field.PlaceWall(wallPosition);
        if (!BothPlayersHaveWayToLastLine())
        {
            _field.RemoveWall(wallPosition);
            var wallDirection = wallPosition.Direction == WallDirection.Horizontal ? "Horizontal" : "Vertical";
            throw new WallBlocksPathForPlayerException(
                $"{wallDirection} wall at {wallPosition.TopLeftCell} blocks way for players");
        }
        DecrementPlayerWallAmount(playerPlacing);
        SwitchCurrentPlayer();
        PlayerPlacedWallEvent?.Invoke(new PlayerPlacedWallEventArgs(playerPlacing, wallPosition));
    }

    private bool PlayerHasWalls(PlayerNumber playerNumber)
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
    /// <summary>Checks if player is on last(relative to his start position)</summary>
    private bool IsOnWinningPosition(PlayerNumber player)
    {
        CellPosition playerPosition = GetPlayerPosition(player);
        if (player == PlayerNumber.First)
        {
            return playerPosition.Y == 0;
        }
        return playerPosition.Y == GameConstants.FieldSize;
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

    public CellPosition GetPlayerPosition(PlayerNumber playerNumber)
    {
        return playerNumber == PlayerNumber.First ? Player1Position : Player2Position;
    }
}
}