using System;
using System.Linq;
using Model;
using Model.Api;
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

        public void HandleGameStartedEvent(object sender, EventArgs args)
        {
            Console.Out.WriteLine("here");
            GameStared = true;
        }

        public void HandleGameEndedEvent(object sender, EventArgs args)
        {
            GameEnded = true;
        }

        public void HandlePlayerWonEvent(object sender, PlayerWonEventArgs args)
        {
            GameWinner = args.WinnerNumber;
        }

        public void HandlePlayerMovedEvent(object sender, PlayerMovedEventArgs args)
        {
            LastMove = new Tuple<PlayerNumber, CellPosition>(args.PlayerNumber, args.Position);
        }

        public void HandlePlayerPlacedWallEvent(object sender, PlayerPlacedWallEventArgs args)
        {
            LastWallPlacement = new Tuple<PlayerNumber, WallPosition>(args.PlayerNumber, args.Position);
        }
    }
    
    [TestFixture]
    public class EventsTests
    {
        private MyEventHandler _player1Handler;
        private MyEventHandler _player2Handler;
        private IGameModel _gameModel;
        
        [SetUp]
        public void Setup()
        {
            _player1Handler = new MyEventHandler();
            _player2Handler = new MyEventHandler();
            _gameModel = new GameModel(_player1Handler, _player2Handler);
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
            int fieldMiddle = _gameModel.FieldMiddle;
            int fieldSize = _gameModel.FieldSize;
            int player1Moves = fieldSize - 1;
            int player2Moves = player1Moves - 1;
            var lastPlayer1Position = _gameModel.Player1Position;
            var lastPlayer2Position = _gameModel.Player2Position;
            foreach (int moveNumber in Enumerable.Range(0, player1Moves + player2Moves))
            {
                if (moveNumber % 2 == 0)
                {
                    int i = moveNumber / 2;
                    var player1MovePosition = lastPlayer1Position.Shifted(0, -1);
                    _gameModel.MovePlayer(PlayerNumber.First, player1MovePosition);
                    lastPlayer1Position = player1MovePosition;
                }
                else
                {
                    //TODO replace with constant
                    var player2StartPosition = new CellPosition(fieldMiddle, 0);
                    var position1 = new CellPosition(fieldMiddle + 1, 0);
                    var position2 = new CellPosition(fieldMiddle + 1, 1);
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
            var movePosition = new CellPosition(_gameModel.FieldMiddle, _gameModel.FieldSize - 2);
            _gameModel.MovePlayer(PlayerNumber.First, movePosition);
            var lastMove = new Tuple<PlayerNumber, CellPosition>(PlayerNumber.First, movePosition);
            Assert.AreEqual(_player1Handler.LastMove, lastMove);
            Assert.AreEqual(_player2Handler.LastMove, lastMove);
        }
        
        [Test]
        public void Players_receive_player2_move()
        {
            _gameModel.StartNewGame();
            var movePosition1 = new CellPosition(_gameModel.FieldMiddle, _gameModel.FieldSize - 2);
            var movePosition2 = new CellPosition(_gameModel.FieldMiddle, 1);
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