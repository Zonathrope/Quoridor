using System.Collections.Generic;
using Model.DataTypes;

namespace Model
{
    public interface IGameModel
    {
        void StartNewGame();
        /// <exception cref="IncorrectPlayerPositionException">Caller pass invalid position.</exception>
        /// <exception cref="CellAlreadyTakenException">Caller tries to move to taken cell.</exception>
        /// <exception cref="AnotherPlayerTurnException">Caller tries to move to taken cell.</exception>
        void MovePlayer(PlayerNumber playerNumber, CellPosition newPosition);
        /// <exception cref="NoWallsLeftException">Player has no walls.</exception>
        /// <exception cref="IncorrectWallPositionException">Caller pass invalid position.</exception>
        /// <exception cref="WallPlaceTakenException">Caller tries to place wall over existing wall.</exception>
        /// <exception cref="WallBlocksPathForPlayerException">Caller tries to place wall that blocks way.</exception>
        void PlaceWall(PlayerNumber playerPlacing, WallPosition wallPosition);
        List<CellPosition> GetCellsAvailableForMove(PlayerNumber playerNumber);
        CellPosition GetPlayerPosition(PlayerNumber playerNumber);
        CellPosition Player1Position{ get; }
        CellPosition Player2Position{ get; }
        int Player1WallAmount { get; }
        int Player2WallAmount { get; }
        List<WallPosition> PlacedWalls { get; }
    }
}