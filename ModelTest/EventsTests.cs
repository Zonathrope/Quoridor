using System;
using System.Linq;
using Model;
using Model.DataTypes;
using NUnit.Framework;

namespace ModelTest
{
    class MyEventHandler: IPlayerView
    {
        public bool GameStared { set; get; }
        public bool GameEnded { set; get; }
        public PlayerNumber GameWinner { set; get; }
        public Tuple<PlayerNumber, CellPosition> LastMove { set; get; }
        public Tuple<PlayerNumber, WallPosition> LastWallPlacement { set; get; }

        public void HandleGameStartedEvent()
        {
            GameStared = true;
        }

        public void HandleGameEndedEvent()
        {
            GameEnded = true;
        }

        public void HandlePlayerWonEvent(PlayerNumber winnerNumber)
        {
            GameWinner = winnerNumber;
        }

        public void HandlePlayerMovedEvent(PlayerMovedEventArgs args)
        {
            LastMove = new Tuple<PlayerNumber, CellPosition>(args.PlayerNumber, args.Position);
        }

        public void HandlePlayerPlacedWallEvent(PlayerPlacedWallEventArgs args)
        {
            LastWallPlacement = new Tuple<PlayerNumber, WallPosition>(args.PlayerNumber, args.WallPosition);
        }
    }
    // TODO maybe separate tests abstract class
    [TestFixture]
    public class EventsTests
    {
        private MyEventHandler _player1Handler;
        private MyEventHandler _player2Handler;
        private CellPosition _player1StartPos;
        private CellPosition _player2StartPos;
        private IGameModel _gameModel;
        
        [SetUp]
        public void Setup()
        {
            _player1Handler = new MyEventHandler();
            _player2Handler = new MyEventHandler();
            _gameModel = new GameModel(_player1Handler, _player2Handler);
            _player1StartPos = GameConstants.Player1DefaultPosition;
            _player2StartPos = GameConstants.Player2DefaultPosition;
        }

        [Test]
        public void Players_receive_game_started_event()
        {
            _gameModel.StartNewGame();
            Assert.IsTrue(_player1Handler.GameStared && _player2Handler.GameStared);
        }
        
        [Test]
        public void Players_receive_game_ended_event()
        {
            _gameModel.StartNewGame();
            _gameModel.EndGame();
            Assert.IsTrue(_player1Handler.GameEnded && _player2Handler.GameEnded);
        }

        [Test]
        public void Players_receive_player_won_event()
        {
            _gameModel.StartNewGame();
            MovePlayer1ToFinish();
            Assert.IsTrue(
                _player1Handler.GameWinner == PlayerNumber.First && 
                _player2Handler.GameWinner == PlayerNumber.First); 
        }

        private void MovePlayer1ToFinish()
        {
            int fieldSize = GameConstants.FieldSize;
            int player1MoveAmount = fieldSize - 1;
            int player2MoveAmount = player1MoveAmount - 1;
            var lastPlayer1Position = _gameModel.Player1Position;
            var lastPlayer2Position = _gameModel.Player2Position;
            foreach (int moveNumber in Enumerable.Range(0, player1MoveAmount + player2MoveAmount))
            {
                if (moveNumber % 2 == 0)
                {
                    var player1MovePosition = lastPlayer1Position.Shifted(0, -1);
                    _gameModel.MovePlayer(PlayerNumber.First, player1MovePosition);
                    lastPlayer1Position = player1MovePosition;
                }
                else
                {
                    var player2StartPosition = _player2StartPos;
                    CellPosition position1 = player2StartPosition.Shifted(1, 0);
                    CellPosition position2 = player2StartPosition.Shifted(1, 1);
                    CellPosition player2MovePosition;
                    if (lastPlayer2Position == player2StartPosition)
                    {
                        player2MovePosition = position1;
                    }
                    else
                    {
                        player2MovePosition = lastPlayer2Position == position1 ? position2 : position1;
                    }
                    _gameModel.MovePlayer(PlayerNumber.Second, player2MovePosition);
                    lastPlayer2Position = player2MovePosition;
                }
            }
        }

        [Test]
        public void Players_receive_player1_move()
        {
            _gameModel.StartNewGame();
            var movePosition = _player1StartPos.Shifted(0, -1);
            _gameModel.MovePlayer(PlayerNumber.First, movePosition);
            var lastMove = new Tuple<PlayerNumber, CellPosition>(PlayerNumber.First, movePosition);
            Assert.AreEqual(_player1Handler.LastMove, lastMove);
            Assert.AreEqual(_player2Handler.LastMove, lastMove);
        }
        
        [Test]
        public void Players_receive_player2_move()
        {
            _gameModel.StartNewGame();
            var movePosition1 = _player1StartPos.Shifted(0, -1);
            var movePosition2 = _player2StartPos.Shifted(0, 1);
            _gameModel.MovePlayer(PlayerNumber.First, movePosition1);
            _gameModel.MovePlayer(PlayerNumber.Second, movePosition2);
            var lastMove = new Tuple<PlayerNumber, CellPosition>(PlayerNumber.Second, movePosition2);
            Assert.AreEqual(_player1Handler.LastMove, lastMove);
            Assert.AreEqual(_player2Handler.LastMove, lastMove);
        }
        
        public void Players_receive_player1_placed_wall()
        {
            _gameModel.StartNewGame();
            var wallPosition = new WallPosition(WallDirection.Horizontal,
                new CellPosition(0, 0), new CellPosition(1, 1));
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition);
            var lastWall = new Tuple<PlayerNumber, WallPosition>(PlayerNumber.First, wallPosition);
            Assert.AreEqual(_player1Handler.LastWallPlacement, lastWall);
            Assert.AreEqual(_player2Handler.LastWallPlacement, lastWall);
        }
        
        [Test]
        public void Players_receive_player2_placed_wll()
        {
            _gameModel.StartNewGame();
            var wallPosition1 = new WallPosition(WallDirection.Horizontal,
                new CellPosition(0, 0), new CellPosition(1, 1));
            var wallPosition2 = new WallPosition(WallDirection.Horizontal,
                new CellPosition(0, 2), new CellPosition(1, 3));
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition1);
            _gameModel.PlaceWall(PlayerNumber.Second, wallPosition2);
            var lastWall = new Tuple<PlayerNumber, WallPosition>(PlayerNumber.Second, wallPosition2);
            Assert.AreEqual(_player1Handler.LastWallPlacement, lastWall);
            Assert.AreEqual(_player2Handler.LastWallPlacement, lastWall);
        }
    }
}