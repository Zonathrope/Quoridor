using NUnit.Framework;
using Moq;

using Quoridor.Model;

namespace ModelTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            var player1Mock = Mock.Of<IPlayerView>();
            var player2Mock = Mock.Of<IPlayerView>();
            IGameModel gameModel = new GameModel(player1Mock, player2Mock);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}