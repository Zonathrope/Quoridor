﻿using System;

namespace Quoridor.Model
{
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
    
    class IncorrectPlayerPositionException : IncorrectUserInputException
    {
        public IncorrectPlayerPositionException(){}
        public IncorrectPlayerPositionException(string message) : base(message) {}
        public IncorrectPlayerPositionException(string message, Exception inner) : base(message, inner){}
    }
    
    class IncorrectWallPositionException : IncorrectUserInputException
    {
        public IncorrectWallPositionException(){}
        public IncorrectWallPositionException(string message) : base(message){}
        public IncorrectWallPositionException(string message, Exception inner) : base(message, inner){}
    }
}