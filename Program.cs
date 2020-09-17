using Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace GarouToremo
{
    class Program
    {
        const int FPS = 60;

        Cheats cheats;
        Timer timer;
        Overlay overlay;

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
            overlay = new Overlay();
        }

        public void Run()
        {
            overlay.Run();
            this.overlay.InfoText = "GarouToremo is running";
            timer = new Timer(CheatLoop, null, 0, FPS);

            string option = String.Empty;
            while (option != "q")
            {
                Console.Clear();
                Console.WriteLine("Enter m to menu or q to exit");

                if (option == "m")
                {
                    ShowMenu();
                }

                option = Console.ReadLine().ToLower();
            }
        }

        private void CheatLoop(Object o)
        {
            cheats.SetHp(Cheats.Player.P1, Cheats.MAX_HP);
            cheats.SetHp(Cheats.Player.P2, Cheats.MAX_HP);
            cheats.SetPower(Cheats.Player.P1, Cheats.MAX_POWER);
            cheats.SetPower(Cheats.Player.P2, Cheats.MAX_POWER);
            cheats.SetTime(Cheats.MAX_TIME);

            byte currentP1Input = cheats.GetCurrentInputByte(Cheats.Player.P1);
            overlay.AddP1Input(currentP1Input);

            byte currentP2Input = cheats.GetCurrentInputByte(Cheats.Player.P2);
            overlay.AddP2Input(currentP2Input);
        }

        private void BackToCenter()
        {
            cheats.SetScenarioPosition(Cheats.POSITION_X_CENTER_SCENARIO);
            cheats.SetPlayerPosition(Cheats.Player.P1, Cheats.POSITION_X_CENTER_P1, Cheats.POSITION_Y_CENTER_P1);
            cheats.SetPlayerPosition(Cheats.Player.P2, Cheats.POSITION_X_CENTER_P2, Cheats.POSITION_Y_CENTER_P2);
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("Enter:");
            Console.WriteLine("1 - Toggle Show Inputs [{0}]", overlay.ShowInputHistory);
            Console.WriteLine("Any other key - Quit menu");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    overlay.ShowInputHistory = !overlay.ShowInputHistory;
                    break;
            }
        }
    }
}
