using Model;
using Model.DataTypes;
using Moq;
using NUnit.Framework;

namespace ModelTest
{
    [TestFixture]
    public class MoveTests
    {
        private IGameModel _gameModel;
        private CellPosition _player1StartPos;
        private CellPosition _player2StartPos;
        [SetUp]
        public void Setup()
        {
            var player1Mock = Mock.Of<IPlayerView>();
            var player2Mock = Mock.Of<IPlayerView>();
            _gameModel = new GameModel(player1Mock, player2Mock);
            _player1StartPos = GameConstants.Player1StartPosition;
            _player2StartPos = GameConstants.Player2StartPosition;
            _gameModel.StartNewGame();
        }
        
        private void TestMove(PlayerNumber playerNumber, CellPosition position)
        {
            _gameModel.MovePlayer(playerNumber, position);
            Assert.AreEqual(_gameModel.GetPlayerPosition(playerNumber), position);
        }

        [Test]
        public void Player1_move_to_correct_position_up()
        {
            TestMove(PlayerNumber.First, _player1StartPos.Shifted(0, -1));
        }
        
        [Test]
        public void Player1_move_to_correct_position_left()
        {
            TestMove(PlayerNumber.First, _player1StartPos.Shifted(-1 , 0));
        }
        
        [Test]
        public void Player1_move_to_correct_position_right()
        {
            TestMove(PlayerNumber.First, _player1StartPos.Shifted(1, 0));
        }
        
        [Test]
        public void Player2_move_to_correct_position_bottom()
        {
            _gameModel.MovePlayer(PlayerNumber.First, _player1StartPos.Shifted(0, -1));
            TestMove(PlayerNumber.Second, _player2StartPos.Shifted(0, 1));
        }
        
        [Test]
        public void Player2_move_to_correct_position_left()
        {
            _gameModel.MovePlayer(PlayerNumber.First, _player1StartPos.Shifted(0, -1));
            TestMove(PlayerNumber.Second, _player2StartPos.Shifted(-1, 0));
        }
        
        [Test]
        public void Player2_move_to_correct_position_right()
        {
            _gameModel.MovePlayer(PlayerNumber.First, _player1StartPos.Shifted(0, -1));
            TestMove(PlayerNumber.Second, _player2StartPos.Shifted(1, 0));
        }
    }
}