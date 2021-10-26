using System;

namespace Model
{
    public class QuoridorException : Exception
    {
        public QuoridorException(string message): base(message){}
        public QuoridorException(string message, Exception inner): base(message, inner){}
    }

    public class IncorrectUserInputException: QuoridorException
    {
        public IncorrectUserInputException(string message): base(message){}
    }

    public class CellAlreadyTakenException : IncorrectUserInputException
    {
        public CellAlreadyTakenException(string message): base(message){}
    }
    
    public class WallPlaceTakenException : IncorrectUserInputException
    {
        public WallPlaceTakenException(String message) : base(message){}
    }
    
    public class IncorrectPlayerPositionException : IncorrectUserInputException
    {
        public IncorrectPlayerPositionException(string message) : base(message) {}
    }
    
    public class IncorrectWallPositionException : IncorrectUserInputException
    {
        public IncorrectWallPositionException(string message) : base(message){}
    }

    public class NoWallsLeftException : IncorrectUserInputException
    {
        public NoWallsLeftException(string message) : base(message){}
    }

    public class WallBlocksPathForPlayerException : IncorrectUserInputException
    {
        public WallBlocksPathForPlayerException(string message) : base(message){}
    }

    public class AnotherPlayerTurnException : IncorrectUserInputException
    {
        public AnotherPlayerTurnException(string message) : base(message){}
    }

    public class NoSuchWallException : QuoridorException
    {
        public NoSuchWallException(string message): base(message){}
    }
}