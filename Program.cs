using Memory;
using SharpDX.DirectInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace GarouToremo
{
    class Program
    {
        const string PROJECT_URL = "https://github.com/alanoliveira/garou-toremo";
        const string PROJECT_NAME = "garou-toremo";
        const string VERSION = "0.0.2-dev";
        const int FPS = 120;

        Cheats cheats;
        InputHandler inputHandler;
        Overlay overlay;
        IHotkeyListenable hotkeyHandler;
        InputHistory p1InputHistory;
        InputHistory p2InputHistory;
        private State state = State.IDLE;
        private Dictionary<int, byte[]> recordedInputSlots = new Dictionary<int, byte[]>();
        private int currentSlot = 0;
        int customP1X = Cheats.POSITION_X_CENTER_P1;
        int customP2X = Cheats.POSITION_X_CENTER_P2;
        int customScenarioX = Cheats.POSITION_X_CENTER_SCENARIO;

        private enum State
        {
            IDLE = 1,
            PREPARING_REC = 2,
            RECORDING = 3,
            PLAYBACKING = 4
        }

        static void Main(string[] args)
        {
            new Program().Run();
        }

        public Program()
        {
            Console.Title = String.Format("{0} - v {1}", PROJECT_NAME, VERSION);
            MemoryHandler mem = new MemoryHandler();

            if (!mem.OpenProcess("Garou")) {
                Console.WriteLine("Error to open Garou proccess. Is the game running?");
                Environment.Exit(1);
            }

            p1InputHistory = new InputHistory();
            p2InputHistory = new InputHistory();
            cheats = new Cheats(mem);
            inputHandler = new InputHandler(mem);
            overlay = new Overlay();
        }

        public void Run()
        {
            overlay.Run();
            this.overlay.BotInfoText = "GarouToremo is running";
            this.overlay.TopInfoText = String.Format("Project URL: {0}", PROJECT_URL);

            Thread cheatLoop = new Thread(this.CheatLoop);
            cheatLoop.Start();

            ShowMenu();
            Environment.Exit(0);
        }

        private void CheatLoop(Object o)
        {
            while (true)
            {
                Thread.Sleep(50);
                cheats.SetHp(Player.P1, Cheats.MAX_HP);
                cheats.SetHp(Player.P2, Cheats.MAX_HP);
                cheats.SetPower(Player.P1, Cheats.MAX_POWER);
                cheats.SetPower(Player.P2, Cheats.MAX_POWER);
                cheats.SetTime(Cheats.MAX_TIME);

                byte currentP1Input = inputHandler.GetCurrentInputByte(Player.P1);
                p1InputHistory.AddInput(currentP1Input);
                overlay.effectiveInputsP1 = p1InputHistory.GetEffectiveInputs();

                byte currentP2Input = inputHandler.GetCurrentInputByte(Player.P2);
                p2InputHistory.AddInput(currentP2Input);
                overlay.effectiveInputsP2 = p2InputHistory.GetEffectiveInputs();


                if (hotkeyHandler != null)
                {
                    hotkeyHandler.Update();

                    if (hotkeyHandler.ResetPositionCenterPressed())
                    {
                        overlay.BotInfoText = "Reset position - center";
                        SetPlayersXPoistion(Cheats.POSITION_X_CENTER_P1, Cheats.POSITION_X_CENTER_P2, Cheats.POSITION_X_CENTER_SCENARIO);
                    }

                    if (hotkeyHandler.ResetPositionLeftPressed())
                    {
                        overlay.BotInfoText = "Reset position - left";
                        SetPlayersXPoistion(Cheats.POSITION_X_MIN + 10, Cheats.POSITION_X_MIN + 100, Cheats.POSITION_X_MIN_SCENARIO);
                    }

                    if (hotkeyHandler.ResetPositionRightPressed())
                    {
                        overlay.BotInfoText = "Reset position - right";
                        SetPlayersXPoistion(Cheats.POSITION_X_MAX - 100, Cheats.POSITION_X_MAX - 10, Cheats.POSITION_X_MAX_SCENARIO);
                    }

                    if (hotkeyHandler.ResetPositionCustomPressed())
                    {
                        overlay.BotInfoText = "Reset position - custom";
                        SetPlayersXPoistion(customP1X, customP2X, customScenarioX);
                    }

                    if (hotkeyHandler.SaveCustomPositionPressed())
                    {
                        overlay.BotInfoText = "Current position saved";
                        customP1X = cheats.GetPosition(Player.P1)[0];
                        customP2X = cheats.GetPosition(Player.P2)[0];
                        customScenarioX = cheats.GetScenarioPosition(Player.P2)[0];
                    }

                    if (hotkeyHandler.ToggleRecordPressed())
                    {
                        if (state == State.IDLE)
                        {
                            state = State.PREPARING_REC;
                            inputHandler.InvertControls();
                            overlay.BotInfoText = "Controls inverted";
                        }
                        else if (state == State.PREPARING_REC)
                        {
                            state = State.RECORDING;
                            inputHandler.StartRecordInput();
                            overlay.BotInfoText = String.Format("Record started on slot #{0}", currentSlot);
                        }
                        else if (state == State.RECORDING)
                        {
                            state = State.IDLE;
                            inputHandler.StopRecordInput();
                            recordedInputSlots[currentSlot] = inputHandler.GetRecordedInput();
                            inputHandler.InvertControls();
                            overlay.BotInfoText = String.Format("Input saved on slot #{0}", currentSlot);
                        }
                        Thread.Sleep(300);
                    }

                    if (hotkeyHandler.TogglePlaybackPressed())
                    {
                        if (state == State.IDLE)
                        {
                            if (!recordedInputSlots.ContainsKey(currentSlot))
                            {
                                overlay.BotInfoText = String.Format("There is no input on slot #{0}", currentSlot);
                            }
                            else
                            {
                                state = State.PLAYBACKING;
                                inputHandler.StartPlaybackInput(recordedInputSlots[currentSlot]);
                                overlay.BotInfoText = String.Format("Playbacking started on slot #{0}", currentSlot);
                            }
                        }
                        else if (state == State.PLAYBACKING)
                        {
                            state = State.IDLE;
                            inputHandler.StopPlaybackInput();
                            overlay.BotInfoText = "Playback Stoped";
                        }
                        Thread.Sleep(300);
                    }
                }
            }
        }

        private void SetPlayersXPoistion(int p1X, int p2X, int scenarionX)
        {
            byte[][] originalAddresses = inputHandler.DisableControls();
            cheats.SetScenarioPosition(scenarionX);
            cheats.SetPlayerPosition(Player.P1, p1X, Cheats.POSITION_Y_CENTER_P1);
            cheats.SetPlayerPosition(Player.P2, p2X, Cheats.POSITION_Y_CENTER_P2);
            Thread.Sleep(50);
            cheats.SetScenarioPosition(scenarionX);
            cheats.SetPlayerPosition(Player.P1, p1X, Cheats.POSITION_Y_CENTER_P1);
            cheats.SetPlayerPosition(Player.P2, p2X, Cheats.POSITION_Y_CENTER_P2);
            Thread.Sleep(50);
            cheats.SetScenarioPosition(scenarionX);
            cheats.SetPlayerPosition(Player.P1, p1X, Cheats.POSITION_Y_CENTER_P1);
            cheats.SetPlayerPosition(Player.P2, p2X, Cheats.POSITION_Y_CENTER_P2);
            Thread.Sleep(400);
            inputHandler.ReenableControls(originalAddresses[0], originalAddresses[1]);
        }

        private void ShowMenu()
        {
            string option = "";
            while (option != "q")
            {
                Console.Clear();
                Console.WriteLine("Get the latest version at {0}", PROJECT_URL);
                Console.WriteLine("Enter:");
                Console.WriteLine("1 - Toggle Show Inputs [{0}]", overlay.ShowInputHistory);
                Console.WriteLine("2 - Set hotkeys");
                Console.WriteLine("3 - Set record slot [{0}]", currentSlot);
                Console.WriteLine("q - Quit");
                option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        overlay.ShowInputHistory = !overlay.ShowInputHistory;
                        break;
                    case "2":
                        SetHotkeys();
                        break;
                    case "3":
                        SetRecordSlot();
                        break;
                }
            }
        }

        private void SetHotkeys()
        {
            IHotkeyListenable newHotkeyHandler = ChooseHotkeyHandler();
            if(newHotkeyHandler != null)
            {
                if(hotkeyHandler != null)
                {
                    hotkeyHandler.Dispose();
                }
                hotkeyHandler = newHotkeyHandler;
                ConfigureHotkeyHandler();
            }
        }

        private IHotkeyListenable ChooseHotkeyHandler()
        {
            Console.Clear();

            Console.WriteLine("1 - Keyboard");
            for(int i = 0; i < ControllerHotkey.DeviceCount(); i++)
            {
                Console.WriteLine("{0} - Joystick {1}", i+2, i);
            }
            string option = Console.ReadLine();

            int intOption = int.Parse(option);
            if (intOption == 1)
            {
                return new KeyboardHotkey();
            } 
            else
            {
                return new ControllerHotkey(intOption-2);
            }

            return null;
        }

        private void ConfigureHotkeyHandler()
        {
            Console.WriteLine("Set reset position Hotkey");
            hotkeyHandler.SetRestPositionHotkey();
            Console.WriteLine("Set save custom position Hotkey");
            hotkeyHandler.SetSaveCustomPositionHotkey();
            Console.WriteLine("Set record input Hotkey");
            hotkeyHandler.SetToggleRecordHotkey();
            Console.WriteLine("Set playback input Hotkey");
            hotkeyHandler.SetTogglePlaybackHotkey();
        }

        private void SetRecordSlot()
        {
            Console.WriteLine("Choose the slot [0-9]");
            int option = int.Parse(Console.ReadLine());
            if(option >= 0 && option <= 9)
            {
                currentSlot = option;
            }
        }
    }
}
