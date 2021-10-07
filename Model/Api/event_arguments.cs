using System;
using Model.DataTypes;

namespace Model.Api
{
    public class PlayerMovedEventArgs : EventArgs
    {
        public PlayerNumber PlayerNumber { get; }
        public CellPosition Position { get; }
        public PlayerMovedEventArgs(PlayerNumber playerNumber, CellPosition newPosition)
        {
            PlayerNumber = playerNumber;
            Position = newPosition;
        }
    }
    
    public class PlayerPlacedWallEventArgs : EventArgs
    {
        public PlayerNumber PlayerNumber { get; }
        public WallPosition Position { set; get; }
        public PlayerPlacedWallEventArgs(PlayerNumber playerNumber, WallPosition wallPosition)
        {
            PlayerNumber = playerNumber;
            Position = wallPosition;
        }
    }
    
    public class PlayerWonEventArgs : EventArgs
    {
        public PlayerNumber WinnerNumber { get; }
        public PlayerWonEventArgs(PlayerNumber playerNumber)
        {
            WinnerNumber = playerNumber;
        }
    }
}