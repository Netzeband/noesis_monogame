using System;

namespace NoesisMonogame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Game ...");
            
            using var game = new Game1();
            game.Run();
            
            Console.WriteLine("Exit Game ...");
        }
    }
}