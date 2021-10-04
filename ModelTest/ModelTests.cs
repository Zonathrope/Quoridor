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

        [Test]
        public void Player1_can_place_horizontal_wall()
        {
            WallPosition wallPosition = new WallPosition(WallDirection.Horizontal,
                new CellPosition(0, 0), new CellPosition(1, 1));
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition);
            Assert.IsTrue(_gameModel.PlacedWalls.Contains(wallPosition));
        }

        [Test]
        public void Player1_can_place_vertical_wall()
        {
            WallPosition wallPosition = new WallPosition(WallDirection.Vertical,
                new CellPosition(0, 0), new CellPosition(1, 1));
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition);
            Assert.IsTrue(_gameModel.PlacedWalls.Contains(wallPosition));
        }

        [Test]
        public void Player2_can_place_horizontal_wall()
        {
            WallPosition wallPosition = new WallPosition(WallDirection.Horizontal,
                new CellPosition(0, 0), new CellPosition(1, 1));
            //TODO introduce constants for field size
            _gameModel.MovePlayer(PlayerNumber.First, 4, 7);
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition);
            Assert.IsTrue(_gameModel.PlacedWalls.Contains(wallPosition));
        }

        [Test]
        public void Player2_can_place_vertical_wall()
        {
            WallPosition wallPosition = new WallPosition(WallDirection.Vertical,
                new CellPosition(0, 0), new CellPosition(1, 1));
            _gameModel.MovePlayer(PlayerNumber.First, 4, 7);
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition);
            Assert.IsTrue(_gameModel.PlacedWalls.Contains(wallPosition));
        }

        [Test]
        public void Player1_cant_place_wall_on_already_placed_door()
        {
            var topLeftCell = new CellPosition(0, 0);
            var bottomRightCell = new CellPosition(1, 1);
            var wallPosition1 = new WallPosition(WallDirection.Horizontal, topLeftCell, bottomRightCell);
            var wallPosition2 = new WallPosition(WallDirection.Vertical, topLeftCell, bottomRightCell);
            _gameModel.MovePlayer(PlayerNumber.First, 4, 7);
            _gameModel.PlaceWall(PlayerNumber.Second, wallPosition1);
            Assert.Throws<WallPlaceTakenException>(
                () => _gameModel.PlaceWall(PlayerNumber.First, wallPosition2));
        }

        [Test]
        public void Player1_cant_place_wall_over_already_placed_door()
        {
            var topLeftCell = new CellPosition(0, 0);
            var bottomRightCell = new CellPosition(1, 1);
            var wallPosition = new WallPosition(WallDirection.Horizontal, topLeftCell, bottomRightCell);
            _gameModel.MovePlayer(PlayerNumber.First, 4, 7);
            _gameModel.PlaceWall(PlayerNumber.Second, wallPosition);
            Assert.Throws<WallPlaceTakenException>(
                () => _gameModel.PlaceWall(PlayerNumber.First, wallPosition));
        }

        //TODO write tests with random wall position pick
        [Test]
        public void Player2_cant_place_wall_on_already_placed_door()
        {
            var topLeftCell = new CellPosition(0, 0);
            var bottomRightCell = new CellPosition(1, 1);
            var wallPosition1 = new WallPosition(WallDirection.Horizontal, topLeftCell, bottomRightCell);
            var wallPosition2 = new WallPosition(WallDirection.Vertical, topLeftCell, bottomRightCell);
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition1);
            Assert.Throws<WallPlaceTakenException>(
                () => _gameModel.PlaceWall(PlayerNumber.Second, wallPosition2));
        }

        [Test]
        public void Player2_cant_place_wall_over_already_placed_door()
        {
            var topLeftCell = new CellPosition(0, 0);
            var bottomRightCell = new CellPosition(1, 1);
            var wallPosition = new WallPosition(WallDirection.Vertical, topLeftCell, bottomRightCell);
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition);
            Assert.Throws<WallPlaceTakenException>(
                () => _gameModel.PlaceWall(PlayerNumber.Second, wallPosition));
        }

        [Test]
        public void Players_cant_block_way_for_player1_by_walls()
        {
            int fieldMiddle = _gameModel.FieldMiddle;
            _gameModel.PlaceWall(PlayerNumber.First,
                new WallPosition(WallDirection.Vertical,
                    new CellPosition(fieldMiddle - 1, 0),
                    new CellPosition(fieldMiddle, 1)));
            _gameModel.PlaceWall(PlayerNumber.Second,
                new WallPosition(WallDirection.Vertical,
                    new CellPosition(fieldMiddle + 1, 0),
                    new CellPosition(fieldMiddle + 2, 1)));
            Assert.Throws<WallBlocksPathForPlayerException>(() =>
            {
                _gameModel.PlaceWall(PlayerNumber.First,
                    new WallPosition(WallDirection.Horizontal,
                        new CellPosition(fieldMiddle, 0),
                        new CellPosition(fieldMiddle + 1, 1)));
            });
        }

        [Test]
        public void Players_cant_block_way_for_player2_by_walls()
        {
            int fieldMiddle = _gameModel.FieldMiddle;
            int fieldEnd = _gameModel.FieldSize - 1;
            _gameModel.PlaceWall(PlayerNumber.First,
                new WallPosition(WallDirection.Vertical,
                    new CellPosition(fieldMiddle - 1, fieldEnd - 1),
                    new CellPosition(fieldMiddle, fieldEnd)));
            _gameModel.PlaceWall(PlayerNumber.Second,
                new WallPosition(WallDirection.Vertical,
                    new CellPosition(fieldMiddle + 1, fieldEnd - 1),
                    new CellPosition(fieldMiddle + 2, fieldEnd)));
            Assert.Throws<WallBlocksPathForPlayerException>(() =>
            {
                _gameModel.PlaceWall(PlayerNumber.First,
                    new WallPosition(WallDirection.Horizontal,
                        new CellPosition(fieldMiddle, fieldEnd - 1),
                        new CellPosition(fieldMiddle + 1, fieldEnd)));
            });
        }
    }
}