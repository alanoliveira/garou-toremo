using Memory;
using System;
using System.Threading;

namespace GarouToremo
{
    class Program
    {
        const int FPS = 60;

        Cheats cheats;
        Timer timer;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        public Program()
        {
            Mem mem = new Mem();

            if (!mem.OpenProcess("Garou")) {
                Console.WriteLine("Error to open Garou proccess. Is the game running?");
                Environment.Exit(1);
            }

            cheats = new Cheats(mem);
        }

        public void Run()
        {
            timer = new Timer(CheatLoop, null, 0, FPS);
            Console.WriteLine("Press q to exit");
            while (Console.ReadKey().Key != ConsoleKey.Q);
        }

        private void CheatLoop(Object o)
        {
            cheats.SetHp(Cheats.Player.P1, Cheats.MAX_HP);
            cheats.SetHp(Cheats.Player.P2, Cheats.MAX_HP);
            cheats.SetPower(Cheats.Player.P1, Cheats.MAX_POWER);
            cheats.SetPower(Cheats.Player.P2, Cheats.MAX_POWER);
            cheats.SetTime(Cheats.MAX_TIME);
        }
    }
}
