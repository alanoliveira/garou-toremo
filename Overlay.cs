using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace GarouToremo
{
    class Overlay : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        const string GAROU_WINDOW_NAME = "Garou: Mark of the Wolves";
        const int FPS = 60;
        const int INFO_TEXT_TIME = 5 * FPS;
        const int DEFAULT_FONT_SIZE = 20;
        const int INPUT_QUEUE_SIZE = 50;

        private readonly StickyWindow window;

        private readonly IntPtr garouWindow;
        private readonly Dictionary<string, Font> fonts;
        private readonly Dictionary<string, SolidBrush> brushes;

        private int infoTextTimer;
        private string _infoText;
        public string InfoText
        {
            get { return _infoText; }
            set {
                infoTextTimer = INFO_TEXT_TIME;
                _infoText = value; 
            }
        }

        public bool ShowInputHistory = true;
        private FixedSizedQueue<byte> p1InputHistory;
        private FixedSizedQueue<byte> p2InputHistory;


        public Overlay()
        {
            this.garouWindow = FindWindow(null, GAROU_WINDOW_NAME);
            if (this.garouWindow == IntPtr.Zero) {
                throw new Exception("Garou window not found");
            }

            this.p1InputHistory = new FixedSizedQueue<byte>(INPUT_QUEUE_SIZE);
            this.p2InputHistory = new FixedSizedQueue<byte>(INPUT_QUEUE_SIZE);

            this.brushes = new Dictionary<string, SolidBrush>();
            this.fonts = new Dictionary<string, Font>();

            Graphics gfx = new Graphics();

            this.window = new StickyWindow(garouWindow, gfx);
            window.FPS = FPS;
            window.IsTopmost = true;
            window.IsVisible = true;

            window.DestroyGraphics += DestroyWindowGraphics;
            window.DrawGraphics += DrawWindowGraphics;
            window.SetupGraphics += SetupWindowGraphics;
        }

        private void SetupWindowGraphics(object sender, SetupGraphicsEventArgs e)
        {
            Graphics gfx = e.Graphics;

            this.brushes["green"] = gfx.CreateSolidBrush(0, 255, 0);
            this.brushes["magenta"] = gfx.CreateSolidBrush(255, 0, 255);
            this.brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
            this.brushes["black"] = gfx.CreateSolidBrush(0, 0, 0);

            if (e.RecreateResources) return;

            this.fonts["arial"] = gfx.CreateFont("Arial", 12);

        }

        private void DrawWindowGraphics(object sender, DrawGraphicsEventArgs e)
        {
            this.window.PlaceAbove(this.garouWindow);

            Graphics gfx = e.Graphics;

            gfx.ClearScene();

            DrawInfoText(gfx);
            if(ShowInputHistory)
            {
                DrawP1InputHistory(gfx);
                DrawP2InputHistory(gfx);
            }
        }

        private void DrawInfoText(Graphics gfx)
        {
            if(this.infoTextTimer > 0) {
                gfx.DrawText(this.fonts["arial"], this.brushes["green"], 30, window.Height - 30, InfoText);
                this.infoTextTimer--;
            }
        }

        private void DrawP1InputHistory(Graphics gfx)
        {
            byte[] history = p1InputHistory.Reverse().ToArray();
            int i = 0;
            foreach(byte input in history)
            {
                if (input == Cheats.INPUT_NEUTRAL)
                {
                    continue;
                }
                string inputString = InputToString(input);
                int x = 30;
                int y = (this.window.Height - 50) - (i * 30);
                DrawInputHistoryInput(gfx, inputString, x, y);
                i++;
            }
        }

        private void DrawP2InputHistory(Graphics gfx)
        {
            byte[] history = p2InputHistory.Reverse().ToArray();
            int i = 0;
            foreach (byte input in history)
            {
                if (input == Cheats.INPUT_NEUTRAL)
                {
                    continue;
                }
                string inputString = InputToString(input);
                Point measure = gfx.MeasureString(this.fonts["arial"], inputString);
                int x = window.Width - 30 - (int)measure.X;
                int y = (this.window.Height - 50) - (i * 30);

                DrawInputHistoryInput(gfx, inputString, x, y);
                i++;
            }
        }

        private void DrawInputHistoryInput(Graphics gfx, string text, int x, int y)
        {
            gfx.DrawText(this.fonts["arial"], this.brushes["magenta"], x+2, y+2, text);
        }

        private string InputToString(int input)
        {
            List<string> inputStrings = new List<string> { };

            int directionalInput = input | 0xF0;
            switch (directionalInput)
            {
                case Cheats.INPUT_UP & Cheats.INPUT_LEFT:
                    inputStrings.Add("⇖");
                    break;
                case Cheats.INPUT_UP & Cheats.INPUT_RIGHT:
                    inputStrings.Add("⇗");
                    break;
                case Cheats.INPUT_DOWN & Cheats.INPUT_LEFT:
                    inputStrings.Add("⇙");
                    break;
                case Cheats.INPUT_DOWN & Cheats.INPUT_RIGHT:
                    inputStrings.Add("⇘");
                    break;
                case Cheats.INPUT_UP:
                    inputStrings.Add("⇑");
                    break;
                case Cheats.INPUT_DOWN:
                    inputStrings.Add("⇓");
                    break;
                case Cheats.INPUT_LEFT:
                    inputStrings.Add("⇐");
                    break;
                case Cheats.INPUT_RIGHT:
                    inputStrings.Add("⇒");
                    break;
            }

            int buttonlInput = input | 0x0F;
            if ((buttonlInput | Cheats.INPUT_HP) == Cheats.INPUT_HP) inputStrings.Add("C");
            if ((buttonlInput | Cheats.INPUT_HK) == Cheats.INPUT_HK) inputStrings.Add("D");
            if ((buttonlInput | Cheats.INPUT_LP) == Cheats.INPUT_LP) inputStrings.Add("A");
            if ((buttonlInput | Cheats.INPUT_LK) == Cheats.INPUT_LK) inputStrings.Add("B");

            return String.Join(' ', inputStrings);
        }

        public void AddP1Input(byte input)
        {
            if (p1InputHistory.Count == 0 || p1InputHistory.Last != input)
                p1InputHistory.Enqueue(input);
        }
                         
        public void AddP2Input(byte input)
        {
            if (p2InputHistory.Count == 0 || p2InputHistory.Last != input)
                p2InputHistory.Enqueue(input);
        }

        private void DestroyWindowGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            foreach (var pair in this.brushes) pair.Value.Dispose();
            foreach (var pair in this.fonts) pair.Value.Dispose();
        }

        public void Run()
        {
            this.window.Create();
        }

        ~Overlay()
        {
            Dispose(false);
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                this.window.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
