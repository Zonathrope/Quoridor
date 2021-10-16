﻿using System;
using System.Linq;
using Model;
using Model.DataTypes;

namespace Controller
{
    class GameController
    {
        private PlayerNumber _playerNumber;
        private readonly IGameModel _gameModel;
        private int _gamemode;
        private readonly Drawer _drawer = new();
        private string[,] GameBoard { get; }

        public void StartGame()
        {
            _playerNumber = PlayerNumber.First;
            _drawer.ShowStartInfo();
            _gameModel.StartNewGame();
            ChooseGameMode();
        }

        public GameController(IGameModel model)
        {
            this._gameModel = model;
            GameBoard = new string[17, 17];
        }

        private void UpdateBoard()
        {
            for (var i = 0; i < 17; i++)
            {
                for (var j = 0; j < 17; j++)
                {
                    if (i % 2 == 0 && j % 2 == 0)
                    {
                        if (i == _gameModel.Player1Position.Y * 2 && j == _gameModel.Player1Position.X * 2)
                        {
                            GameBoard[i, j] = DrawConstants.Player1;
                        } else if (i == _gameModel.Player2Position.Y * 2 && j == _gameModel.Player2Position.X * 2)
                        {
                            GameBoard[i, j] = DrawConstants.Player2;
                        }
                        else
                        {
                            GameBoard[i,j] = (DrawConstants.EmptyCell);
                        }
                    }

                    if ((j % 2 != 0) || (i % 2 != 0))
                    {
                        GameBoard[i, j] = (DrawConstants.EmptyVWall);
                    }

                    if (j % 2 != 0)
                    {
                        GameBoard[i, j] = (DrawConstants.EmptyHWall);
                    }
                }
            }

            foreach (var (wallOrientation, topLeftCell) in _gameModel.PlacedWalls)
            {
                if (wallOrientation == WallOrientation.Vertical)
                {
                    GameBoard[topLeftCell.Y * 2 + 1, topLeftCell.X * 2] = DrawConstants.HorizontalWall;
                    GameBoard[topLeftCell.Y * 2 + 1, topLeftCell.X * 2 + 1] = DrawConstants.HorizontalWall;
                    GameBoard[topLeftCell.Y * 2 + 1, topLeftCell.X * 2 + 2] = DrawConstants.HorizontalWall;
                }
                else
                {
                    GameBoard[topLeftCell.Y * 2, topLeftCell.X * 2 + 1] = DrawConstants.VerticalWall;
                    GameBoard[topLeftCell.Y * 2 + 1, topLeftCell.X * 2 + 1] = DrawConstants.VerticalWall;
                    GameBoard[topLeftCell.Y * 2 + 2, topLeftCell.X * 2 + 1] = DrawConstants.VerticalWall;
                }
            }
        }
        
        
        
        private void ChooseGameMode()
        {
            UpdateBoard();
            _drawer.ChooseGameMode();
            var playerInput = this.ValidateInput();
            switch (playerInput)
            {
                case "1": //run against bot
                    _gamemode = 1;
                    HandleTurnAgainstBot();
                break;
                case "2": //run against yourself
                    _gamemode = 2;
                    HandleTurn();
                break;
                default:
                    _drawer.ClearConsole();
                    Console.WriteLine("Incorrect Input");
                    ChooseGameMode();
                break;
            }
        }

        private void HandleTurnAgainstBot()
        {
            if (_playerNumber == PlayerNumber.First)
            {
                HandleTurn();
            }
            else
            {
                Random rng = new Random();
                var botChoose = rng.Next(1, 3);
                switch (botChoose)
                {
                case 1:
                    var move = rng.Next(1, _gameModel.GetCellsAvailableForMove(_playerNumber).Count);
                    _gameModel.MovePlayer(_playerNumber, new CellPosition(
                        _gameModel.GetCellsAvailableForMove(_playerNumber)[move - 1].X,
                        _gameModel.GetCellsAvailableForMove(_playerNumber)[move - 1].Y)
                    );
                    _playerNumber = _playerNumber == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;
                    break;
                case 2 when _gameModel.Player2WallAmount > 0:
                    while (true)
                    {
                        try
                        {
                            var orientation = rng.Next(0, 2) == 0 ? WallOrientation.Horizontal : WallOrientation.Vertical;
                            _gameModel.PlaceWall(_playerNumber, new WallPosition(orientation,
                                new CellPosition(rng.Next(0, 9), rng.Next(0, 9))));
                            _playerNumber = _playerNumber == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;
                            break;
                        }
                        catch (Exception e) {}
                    }
                    break;
                }
                HandleTurn();
            }
        }


        private void HandleTurn()
        {
            while (true)
            {
                if (CheckWin())
                {
                    _drawer.DrawWinner(_playerNumber);
                    StartGame();
                    return;
                }
                UpdateBoard();
                _drawer.DrawBoard(GameBoard);
                var isPlayerHasWalls = _playerNumber == PlayerNumber.First
                    ? _gameModel.Player1WallAmount > 0
                    : _gameModel.Player2WallAmount > 0;
                _drawer.DrawTurnOptions(isPlayerHasWalls);
                var input = ValidateInput();
                switch (input)
                {
                    case "1":
                        PlayerMovement();
                        return;
                    case "2" when isPlayerHasWalls:
                        PlaceWall();
                        break;
                }
            }
        }

        private void PlayerMovement()
        {
            while (true)
            {
                try
                {
                    _drawer.DrawBoard(GameBoard);
                    _drawer.DrawMoveOptions(_gameModel.GetCellsAvailableForMove(_playerNumber));
                    var input = Int32.Parse(ValidateInput());
                    _gameModel.MovePlayer(_playerNumber, new CellPosition(
                        _gameModel.GetCellsAvailableForMove(_playerNumber)[input - 1].X,
                        _gameModel.GetCellsAvailableForMove(_playerNumber)[input - 1].Y)
                    );
                    _playerNumber = _playerNumber == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;

                    if (_gamemode == 1)
                    {
                        HandleTurnAgainstBot();
                    }
                    else
                    {
                        HandleTurn();
                    }
                    return;
                }
                catch (Exception e)
                {
                    _drawer.ClearConsole();
                    _drawer.DrawBoard(GameBoard);
                    Console.WriteLine(e.Message);
                    PlayerMovement();
                }
            }
        }

        private void PlaceWall()
        {
            while (true)
            {
                try
                {
                    _drawer.DrawBoard(GameBoard);
                    _drawer.DrawWallOption();
                    var input = ValidateWallPlacementInput();
                    var orientation = input[2] == "V" ? WallOrientation.Horizontal : WallOrientation.Vertical;
                    _gameModel.PlaceWall(_playerNumber, new WallPosition(orientation,
                        new CellPosition(int.Parse(input[0]), int.Parse(input[1]))));
                    _playerNumber = _playerNumber == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;
                    HandleTurn();
                    return;
                }
                catch (Exception e)
                {
                    _drawer.ClearConsole();
                    Console.WriteLine(e.Message);
                    PlaceWall();
                }
            }
        }
        
        private bool CheckWin()
        {
            var pos = _gameModel.Player1Position;
            var pos2 = _gameModel.Player2Position;
            return GameConstants.Player1WinLine.Any(winingPos => winingPos == pos) 
                   || GameConstants.Player2WinLine.Any(winingPos => winingPos == pos2);
        }

        private string[] ValidateWallPlacementInput()
        {
            while (true)
            {
                var input = Console.ReadLine()?.Split(" ");
                if (input?.Length > 0)
                {
                    Console.Clear();
                    return input;
                }
                Console.WriteLine("Empty inputs arent allowed");
            }
        }

        private string ValidateInput()
        {
            while (true)
            {
                var input = Console.ReadLine()?.Trim(Trim.trimValues);
                if (input != "")
                {
                    Console.Clear();
                    return input;
                }
                Console.WriteLine("Empty inputs arent allowed");
            }
        }
    }
}
