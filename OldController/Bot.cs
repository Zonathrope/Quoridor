using System;
using Model;
using Model.DataTypes;

namespace Controller
{
    public class Bot
    {
        public static IGameModel BotTurn(IGameModel model)
        { 
            Random rng = new Random();
                var botChoose = rng.Next(1, 3);
                switch (botChoose)
                {
                    case 1:
                        var move = rng.Next(1, model.GetField().GetCellsForMove(PlayerNumber.Second).Count);
                        model.MovePlayer(PlayerNumber.Second, new CellPosition(
                            model.GetField().GetCellsForMove(PlayerNumber.Second)[move - 1].X,
                            model.GetField().GetCellsForMove(PlayerNumber.Second)[move - 1].Y)
                        );
                        break;
                    case 2 when model.GetField().Player2WallAmount > 0:
                        while (true)
                        {
                            try
                            {
                                var orientation = rng.Next(0, 2) == 0 ? WallOrientation.Horizontal : WallOrientation.Vertical;
                                model.PlaceWall(PlayerNumber.Second, new WallPosition(orientation,
                                    new CellPosition(rng.Next(0, 9), rng.Next(0, 9))));
                                break;
                            }
                            catch (Exception) {}
                        }
                        break;
                }
                return model;
        }
    }
}