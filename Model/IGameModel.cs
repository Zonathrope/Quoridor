﻿using System.Collections.Generic;
using Model.DataTypes;

namespace Model
{
    public interface IGameModel
    {
        void StartNewGame();
        void EndGame();
        void MovePlayer(PlayerNumber playerNumber, CellPosition newPosition);
        void PlaceWall(PlayerNumber playerPlacing, WallPosition wallPosition);
        List<CellPosition> GetCellsAvailableForMove(PlayerNumber playerNumber);
        CellPosition Player1Position{ get; }
        CellPosition Player2Position{ get; }
        int Player1WallAmount { get; }
        int Player2WallAmount { get; }
        CellPosition GetPlayerPosition(PlayerNumber playerNumber);
        List<WallPosition> PlacedWalls { get; }
        //TODO think if i need to move this constants out
        int FieldSize { get; }
        int FieldMiddle { get; }
        CellPosition Player1DefaultPosition { get; }
        CellPosition Player2DefaultPosition { get; }

    }
}