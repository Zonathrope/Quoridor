using System;

namespace Model.Api
{
    public interface IPlayerView
    {
        void HandleGameStartedEvent(object sender, EventArgs args);
        void HandleGameEndedEvent(object sender, EventArgs args);
        void HandlePlayerWonEvent(object sender, PlayerWonEventArgs args);
        void HandlePlayerMovedEvent(object sender, PlayerMovedEventArgs args);
        void HandlePlayerPlacedWallEvent(object sender, PlayerPlacedWallEventArgs args);
    }
}