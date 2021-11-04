using System;
using Model;

namespace AIProject
{
    class Program
    {
        static void Main(string[] args)
        {
            IGameModel gameModel = new GameModel();
            Ai skynet = new Ai(); 
            Console.WriteLine(skynet.Negamax(gameModel.GetField(), 1,-999, +999, 1).Move.ToString());
            //Console.WriteLine("123");
            Console.ReadLine();
        }
    }
}