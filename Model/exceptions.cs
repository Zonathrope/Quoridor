using System;

namespace Model
{
    //TODO make base Quoridor exception
    public class IncorrectUserInputException: Exception
    {
        public IncorrectUserInputException(){}
        public IncorrectUserInputException(string message): base(message){}
        public IncorrectUserInputException(string message, Exception inner): base(message, inner){}
    }

    public class CellAlreadyTakenException : IncorrectUserInputException
    {
        public CellAlreadyTakenException(){}
        public CellAlreadyTakenException(string message): base(message){}
        public CellAlreadyTakenException(string message, Exception inner): base(message, inner){}
    }
    
    public class WallPlaceTakenException : IncorrectUserInputException
    {
        public WallPlaceTakenException(){}
        public WallPlaceTakenException(string message): base(message){}
        public WallPlaceTakenException(string message, Exception inner): base(message, inner){}
    }
    
    public class IncorrectPlayerPositionException : IncorrectUserInputException
    {
        public IncorrectPlayerPositionException(){}
        public IncorrectPlayerPositionException(string message) : base(message) {}
        public IncorrectPlayerPositionException(string message, Exception inner) : base(message, inner){}
    }
    
    public class IncorrectWallPositionException : IncorrectUserInputException
    {
        public IncorrectWallPositionException(){}
        public IncorrectWallPositionException(string message) : base(message){}
        public IncorrectWallPositionException(string message, Exception inner) : base(message, inner){}
    }

    public class NoWallsLeftException : IncorrectUserInputException
    {
        public NoWallsLeftException(){}
        public NoWallsLeftException(string message) : base(message){}
        public NoWallsLeftException(string message, Exception inner) : base(message, inner){}
    }

    public class WallBlocksPathForPlayerException : IncorrectUserInputException
    {
        public WallBlocksPathForPlayerException(){}
        public WallBlocksPathForPlayerException(string message) : base(message){}
        public WallBlocksPathForPlayerException(string message, Exception inner) : base(message, inner){}
    }

    public class AnotherPlayerTurnException : IncorrectUserInputException
    {
        public AnotherPlayerTurnException(){}
        public AnotherPlayerTurnException(string message) : base(message){}
        public AnotherPlayerTurnException(string message, Exception inner) : base(message, inner){}
    }
}