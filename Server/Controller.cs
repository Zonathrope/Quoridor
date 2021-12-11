using System;
using System.Text.Json;
using Model;
using Model.DataTypes;

namespace Server
{
    using RequestResponse = String;
    internal class Controller
    {
        private readonly GameModel _game;

        public Controller()
        {
            _game = new GameModel();
            _game.StartNewGame();
        }

        public RequestResponse HandleRequest(string request)
        {
            Move? move = ParseMove(request);
            ApplyMove(move);
            return GenResponse();
            
        }

        private Move? ParseMove(string requestJson)
        {
            Move? move = null;
            try
            {
                move = JsonSerializer.Deserialize<PlaceWall>(requestJson);
            }
            catch (NotSupportedException) { }
            try
            {
                move = JsonSerializer.Deserialize<Jump>(requestJson);
            }
            catch (NotSupportedException) { }
            try
            {
                move = JsonSerializer.Deserialize<MovePlayer>(requestJson);
            }
            catch (NotSupportedException) { }
            return move;
        }

        private void ApplyMove(Move? move)
        {
            if (move is null)
                return;
            switch (move)
            {
                case MovePlayer movePlayer:
                    _game.MovePlayer(movePlayer.Player, movePlayer.NewPosition);
                    break;
                case Jump jump:
                    _game.MovePlayer(jump.Player, jump.NewPosition);
                    break;
                case PlaceWall placeWall:
                    _game.PlaceWall(placeWall.Placer, placeWall.WallPosition);
                    break;
            }
        }

        private RequestResponse GenResponse()
        {
            return JsonSerializer.Serialize(_game.GetGameState());
        }
    }
}