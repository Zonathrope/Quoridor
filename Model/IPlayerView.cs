using System;
using Model.DataTypes;

namespace Model
{
    public interface IPlayerView
    {
        void HandleGameStartedEvent();
        void HandleGameEndedEvent();
        void HandlePlayerWonEvent(PlayerNumber winnerNumber);
        void HandlePlayerMovedEvent(PlayerMovedEventArgs args);
        void HandlePlayerPlacedWallEvent(PlayerPlacedWallEventArgs args);
    }
}