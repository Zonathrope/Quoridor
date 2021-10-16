using System;
using Quoridor.View.Interfaces;

namespace Quoridor.View
{
    public class View : IView
    {
        private readonly Drawer _drawer = new();
        private readonly char[] _trimValues = {' '};

        public void StartGame()
        {
            _drawer.ShowStartInfo();
            this.ChooseGameMode();
        }

        private string ChooseGameMode()
        {
            var playerInput = this.HandleInput();
            switch (playerInput)
            {
                case "1": //run against bot
                    return "1";
                case "2": //run against yourself
                    return "2";
                default:
                    _drawer.ClearConsole();
                    Console.WriteLine("Incorrect Input");
                    this.ChooseGameMode();
                    return " ";
            }
        }

        private string HandleInput()
        {
            while (true)
            {
                var input = Console.ReadLine()?.Trim(this._trimValues);
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

