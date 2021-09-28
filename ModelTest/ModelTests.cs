using Model.Api;
using NUnit.Framework;
using Moq;

using Model;

namespace ModelTest
{
    [TestFixture]
    public class StarEndTests
    {
        private IGameModel _gameModel;
        [SetUp]
        public void Setup()
        {
            var player1Mock = Mock.Of<IPlayerView>();
            var player2Mock = Mock.Of<IPlayerView>();
            _gameModel = new GameModel(player1Mock, player2Mock);
        }

        [Test]
        public void Game_starts()
        {
            Assert.DoesNotThrow(() => _gameModel.StartNewGame());
        }
        
        [Test]
        public void Game_ends()
        {
            Assert.DoesNotThrow(() => _gameModel.EndGame());
        }
        
        [Test]
        public void Game_starts_and_ends()
        {
            Assert.DoesNotThrow(() =>
            {
                _gameModel.StartNewGame();
                _gameModel.EndGame();                
            });
        }
    }

    [TestFixture]
    public class BasicActionsTest
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
        
        private bool TestMove(PlayerNumber playerNumber, CellPosition position)
        {
            _gameModel.MovePlayer(playerNumber, 4, 7);
            return _gameModel.GetPlayerPosition(playerNumber) == position;
        }

        [Test]
        public void Player1_move_to_correct_position_1()
        {
            Assert.IsTrue(TestMove(PlayerNumber.First, new CellPosition(4, 7)));
        }
        
        [Test]
        public void Player1_move_to_correct_position_2()
        {
            Assert.IsTrue(TestMove(PlayerNumber.First, new CellPosition(3, 8)));
        }
        
        [Test]
        public void Player1_move_to_correct_position_3()
        {
            Assert.IsTrue(TestMove(PlayerNumber.First, new CellPosition(5, 8)));
        }
        
        [Test]
        public void Player2_move_to_correct_position_1()
        {
            _gameModel.MovePlayer(PlayerNumber.First, 4, 7);
            Assert.IsTrue(TestMove(PlayerNumber.Second, new CellPosition(4, 1)));
        }
        
        [Test]
        public void Player2_move_to_correct_position_2()
        {
            _gameModel.MovePlayer(PlayerNumber.First, 4, 7);
            Assert.IsTrue(TestMove(PlayerNumber.Second, new CellPosition(3, 0)));
        }
        
        [Test]
        public void Player2_move_to_correct_position_3()
        {
            _gameModel.MovePlayer(PlayerNumber.First, 4, 7);
            Assert.IsTrue(TestMove(PlayerNumber.Second, new CellPosition(5, 0)));
        }
    }
}