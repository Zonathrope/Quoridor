using Model.Api;
using NUnit.Framework;
using Moq;

using Model;

namespace ModelTest
{
    [TestFixture]
    public class Tests
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

        [Test]
        public void Player1_makes_move()
        {
            
        }
    }
}