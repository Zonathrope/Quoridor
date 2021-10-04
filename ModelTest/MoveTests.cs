using Model;
using Model.Api;
using Moq;
using NUnit.Framework;

namespace ModelTest
{
    [TestFixture]
    public class MoveTests
    {
        private IGameModel _gameModel;
        [SetUp]
        public void Setup()
        {
            var player1Mock = Mock.Of<IPlayerView>();
            var player2Mock = Mock.Of<IPlayerView>();
            _gameModel = new GameModel(player1Mock, player2Mock);
            _gameModel.StartNewGame();
        }
        
        private void TestMove(PlayerNumber playerNumber, CellPosition position)
        {
            _gameModel.MovePlayer(playerNumber, position.X, position.Y);
            Assert.AreEqual(_gameModel.GetPlayerPosition(playerNumber), position);
        }

        [Test]
        public void Player1_move_to_correct_position_1()
        {
            TestMove(PlayerNumber.First, new CellPosition(4, 7));
        }
        
        [Test]
        public void Player1_move_to_correct_position_2()
        {
            TestMove(PlayerNumber.First, new CellPosition(3, 8));
        }
        
        [Test]
        public void Player1_move_to_correct_position_3()
        {
            TestMove(PlayerNumber.First, new CellPosition(5, 8));
        }
        
        [Test]
        public void Player2_move_to_correct_position_1()
        {
            _gameModel.MovePlayer(PlayerNumber.First, 4, 7);
            TestMove(PlayerNumber.Second, new CellPosition(4, 1));
        }
        
        [Test]
        public void Player2_move_to_correct_position_2()
        {
            _gameModel.MovePlayer(PlayerNumber.First, 4, 7);
            TestMove(PlayerNumber.Second, new CellPosition(3, 0));
        }
        
        [Test]
        public void Player2_move_to_correct_position_3()
        {
            _gameModel.MovePlayer(PlayerNumber.First, 4, 7);
            TestMove(PlayerNumber.Second, new CellPosition(5, 0));
        }
    }
}