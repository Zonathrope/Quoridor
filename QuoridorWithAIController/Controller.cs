using System;
using System.Threading;
using Model;
using Model.DataTypes;

namespace QuoridorWithAIController
{
    //TODO replace mocks
    internal class AIMock : IAI
    {
        public Object GetMove()
        {
            throw new System.NotImplementedException();
        }
    }
    
    internal class ViewMock : IView
    {
        public void HandleGameStartedEvent()
        {
            throw new System.NotImplementedException();
        }

        public void HandleGameEndedEvent()
        {
            throw new System.NotImplementedException();
        }

        public void HandlePlayerWonEvent(PlayerNumber winnerNumber)
        {
            throw new System.NotImplementedException();
        }

        public void HandlePlayerMovedEvent(PlayerNumber playerNumber, CellPosition newPosition, bool isJump = false)
        {
            throw new System.NotImplementedException();
        }

        public void HandlePlayerPlacedWallEvent(PlayerNumber playerPlacing, WallPosition wallPosition)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class Controller
    {
        //TODO make more flexible
        private IAI _ai = new AIMock();
        private IView _view = new ViewMock();
        private IGameModel _gameModel;

        public Controller()
        {
            _gameModel = new GameModel(_view);
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}