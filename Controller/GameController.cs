using System;
using System.Linq;
using AIProject;
using Model;
using Model.DataTypes;
using Controller.Constants;
using Model.Internal;

namespace Controller
{
    class GameController
    {
        private PlayerNumber _playerNumber;
        private IGameModel _gameModel;
        private int _gamemode;
        private readonly Drawer _drawer = new();
        private string[,] GameBoard { get; }

        public void StartGame()
        {
            _playerNumber = PlayerNumber.First;
            _gameModel.StartNewGame();
            Ai skynet = new Ai(); 
            Console.WriteLine(skynet.Negamax(_gameModel.GetField(), 1,-999, +999, 1).Move.ToString());
            //Console.WriteLine("123");
            Console.ReadLine();
            _drawer.ShowStartInfo();
            ChooseGameMode();
        }

        public GameController(IGameModel model)
        {
            _gameModel = model;
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
            _drawer.ChooseGameMode();
            var input = ValidateInput();
            switch (input[0])
            {
                case "1": //run against bot
                    _gamemode = 1;
                    HandleTurn();
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
                if (_gamemode == 1 && _playerNumber == PlayerNumber.Second)
                {
                    _gameModel = Bot.BotTurn(_gameModel);
                    _playerNumber = _playerNumber == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;
                    HandleTurn();
                } 
                else 
                {
                    UpdateBoard();
                    _drawer.DrawBoard(GameBoard);
                    var isPlayerHasWalls = _playerNumber == PlayerNumber.First
                        ? _gameModel.GetField().Player1WallAmount > 0
                        : _gameModel.GetField().Player2WallAmount > 0;
                    _drawer.DrawTurnOptions(isPlayerHasWalls);
                    var input = ValidateInput();
                    switch (input[0])
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
        }

        private void PlayerMovement()
        {
            while (true)
            {
                try
                {
                    _drawer.DrawBoard(GameBoard);
                    _drawer.DrawMoveOptions(_gameModel.GetField().GetCellsForMove(_playerNumber));
                    var input = ValidateInput();
                    _gameModel.MovePlayer(_playerNumber, new CellPosition(
                        _gameModel.GetField().GetCellsForMove(_playerNumber)[Int32.Parse(input[0]) - 1].X,
                        _gameModel.GetField().GetCellsForMove(_playerNumber)[Int32.Parse(input[0]) - 1].Y)
                    );
                    _playerNumber = _playerNumber == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;
                    HandleTurn();
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
                    var input = ValidateInput();
                    var orientation = input[2] == "V" ? WallOrientation.Horizontal : WallOrientation.Vertical;
                    _gameModel.PlaceWall(_playerNumber, new WallPosition(orientation,
                        new CellPosition(int.Parse(input[0]), int.Parse(input[1]))));
                    _playerNumber = _playerNumber == PlayerNumber.First ? PlayerNumber.Second : PlayerNumber.First;
                    HandleTurn();
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

        private string[] ValidateInput()
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

        // private string ValidateInput()
        // {
        //     while (true)
        //     {
        //         var input = Console.ReadLine()?.Trim(Trim.trimValues);
        //         if (input != "")
        //         {
        //             Console.Clear();
        //             return input;
        //         }
        //         Console.WriteLine("Empty inputs arent allowed");
        //     }
        // }
    }
}
