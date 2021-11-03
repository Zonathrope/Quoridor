﻿using Model;
using Model.DataTypes;
using Moq;
using NUnit.Framework;

namespace ModelTest
{
    [TestFixture]
    public class WallPlacementTests
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

        [Test]
        public void Player1_can_place_horizontal_wall()
        {
            WallPosition wallPosition = new WallPosition(WallOrientation.Horizontal, new CellPosition(0, 0));
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition);
            Assert.IsTrue(_gameModel.PlacedWalls.Contains(wallPosition));
        }

        [Test]
        public void Player1_can_place_vertical_wall()
        {
            WallPosition wallPosition = new WallPosition(WallOrientation.Vertical,new CellPosition(0, 0));
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition);
            Assert.IsTrue(_gameModel.PlacedWalls.Contains(wallPosition));
        }

        [Test]
        public void Player2_can_place_horizontal_wall()
        {
            WallPosition wallPosition = new WallPosition(WallOrientation.Horizontal, new CellPosition(0, 0));
            //TODO introduce constants for field size
            _gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4, 7));
            _gameModel.PlaceWall(PlayerNumber.Second, wallPosition);
            Assert.IsTrue(_gameModel.PlacedWalls.Contains(wallPosition));
        }

        [Test]
        public void Player2_can_place_vertical_wall()
        {
            WallPosition wallPosition = new WallPosition(WallOrientation.Vertical, new CellPosition(0, 0));
            _gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4, 7));
            _gameModel.PlaceWall(PlayerNumber.Second, wallPosition);
            Assert.IsTrue(_gameModel.PlacedWalls.Contains(wallPosition));
        }

        [Test]
        public void Player1_cant_place_wall_on_already_placed_door()
        {
            var topLeftCell = new CellPosition(0, 0);
            var bottomRightCell = new CellPosition(1, 1);
            var wallPosition1 = new WallPosition(WallOrientation.Horizontal, topLeftCell);
            var wallPosition2 = new WallPosition(WallOrientation.Vertical, topLeftCell);
            _gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4, 7));
            _gameModel.PlaceWall(PlayerNumber.Second, wallPosition1);
            Assert.Throws<WallPlaceTakenException>(
                () => _gameModel.PlaceWall(PlayerNumber.First, wallPosition2));
        }

        [Test]
        public void Player1_cant_place_wall_over_already_placed_door()
        {
            var topLeftCell = new CellPosition(0, 0);
            var bottomRightCell = new CellPosition(1, 1);
            var wallPosition = new WallPosition(WallOrientation.Horizontal, topLeftCell);
            _gameModel.MovePlayer(PlayerNumber.First, new CellPosition(4, 7));
            _gameModel.PlaceWall(PlayerNumber.Second, wallPosition);
            Assert.Throws<WallPlaceTakenException>(
                () => _gameModel.PlaceWall(PlayerNumber.First, wallPosition));
        }

        [Test]
        public void Player2_cant_place_wall_on_already_placed_door()
        {
            var topLeftCell = new CellPosition(0, 0);
            var bottomRightCell = new CellPosition(1, 1);
            var wallPosition1 = new WallPosition(WallOrientation.Horizontal, topLeftCell);
            var wallPosition2 = new WallPosition(WallOrientation.Vertical, topLeftCell);
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition1);
            Assert.Throws<WallPlaceTakenException>(
                () => _gameModel.PlaceWall(PlayerNumber.Second, wallPosition2));
        }

        [Test]
        public void Player2_cant_place_wall_over_already_placed_door()
        {
            var topLeftCell = new CellPosition(0, 0);
            var bottomRightCell = new CellPosition(1, 1);
            var wallPosition = new WallPosition(WallOrientation.Vertical, topLeftCell);
            _gameModel.PlaceWall(PlayerNumber.First, wallPosition);
            Assert.Throws<WallPlaceTakenException>(
                () => _gameModel.PlaceWall(PlayerNumber.Second, wallPosition));
        }

        [Test]
        public void Players_cant_block_way_for_player1_by_walls()
        {
            _gameModel.PlaceWall(PlayerNumber.First,
                new WallPosition(WallOrientation.Vertical,
                    _player2StartPos.Shifted(-1, 0)));
            _gameModel.PlaceWall(PlayerNumber.Second,
                new WallPosition(WallOrientation.Horizontal,
                    _player2StartPos.Shifted(0, 1)));
            Assert.Throws<WallBlocksPathForPlayerException>(() =>
            {
                _gameModel.PlaceWall(PlayerNumber.First,
                    new WallPosition(WallOrientation.Vertical,
                        _player2StartPos.Shifted(1, 0)));
            });
        }

        [Test]
        //TODO make normal comment with wall positions schemes
        public void Players_cant_block_way_for_player2_by_walls()
        {
            var wall1 = new WallPosition(WallOrientation.Vertical, _player1StartPos.Shifted(-2, -1));
            var wall2 = new WallPosition(WallOrientation.Horizontal, _player1StartPos.Shifted(-1, -2));
            var wall3 = new WallPosition(WallOrientation.Horizontal, _player1StartPos.Shifted(1, -2));
            var wall4 = new WallPosition(WallOrientation.Vertical, _player1StartPos.Shifted(2, -1));
            _gameModel.PlaceWall(PlayerNumber.First, wall1);
            _gameModel.PlaceWall(PlayerNumber.Second, wall2);
            _gameModel.PlaceWall(PlayerNumber.First, wall3);
                    Assert.Throws<WallBlocksPathForPlayerException>(() =>
            {
                _gameModel.PlaceWall(PlayerNumber.Second, wall4);
            });
        }
        
        [Test]
        public void Cant_overlap_walls()
        {
            var firstWallPosition = new WallPosition(WallOrientation.Horizontal, new CellPosition(0, 0));
            var secondWallPosition = new WallPosition(WallOrientation.Horizontal, new CellPosition(1, 0));
            _gameModel.PlaceWall(PlayerNumber.First, firstWallPosition);
            Assert.Throws<WallPlaceTakenException>(() =>
            {
                _gameModel.PlaceWall(PlayerNumber.Second, secondWallPosition);
            });
        }
    }
}