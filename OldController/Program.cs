using Model;
using System;

namespace Controller
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = new GameModel();
            var controller = new GameController(model);
            controller.StartGame();
        }
    }
}
