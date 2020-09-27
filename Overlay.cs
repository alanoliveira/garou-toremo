﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace GarouToremo
{
    class Overlay : IDisposable
    {
        private enum AnchorPoint
        {
            LEFT  = 1,
            RIGHT = 2,
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        const string GAROU_WINDOW_NAME = "Garou: Mark of the Wolves";
        const int FPS = 60;
        const int INFO_TEXT_TIME = 5 * FPS;

        private readonly StickyWindow window;

        private readonly IntPtr garouWindow;
        private readonly Dictionary<string, Font> fonts;
        private readonly Dictionary<string, SolidBrush> brushes;

        private int botInfoTextTimer;
        private string _botInfoText;
        public string BotInfoText
        {
            get { return _botInfoText; }
            set {
                botInfoTextTimer = INFO_TEXT_TIME;
                _botInfoText = value; 
            }
        }

        private int topInfoTextTimer;
        private string _topInfoText;
        public string TopInfoText
        {
            get { return _topInfoText; }
            set
            {
                topInfoTextTimer = INFO_TEXT_TIME;
                _topInfoText = value;
            }
        }

        public byte[] effectiveInputsP1 = new byte[] { };
        public byte[] effectiveInputsP2 = new byte[] { };
        public bool ShowInputHistory = true;

        public Overlay()
        {
            this.garouWindow = FindWindow(null, GAROU_WINDOW_NAME);
            if (this.garouWindow == IntPtr.Zero) {
                throw new Exception("Garou window not found");
            }

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
                DrawInputHistory(gfx, effectiveInputsP1, AnchorPoint.LEFT);
                DrawInputHistory(gfx, effectiveInputsP2, AnchorPoint.RIGHT);
            }
        }

        private void DrawInfoText(Graphics gfx)
        {
            if(this.botInfoTextTimer > 0) {
                gfx.DrawTextWithBackground(this.fonts["arial"], this.brushes["green"], this.brushes["black"], 30, window.Height - 30, BotInfoText);
                this.botInfoTextTimer--;
            }

            if (this.topInfoTextTimer > 0)
            {
                gfx.DrawTextWithBackground(this.fonts["arial"], 30, this.brushes["green"], this.brushes["black"], 30, 30, TopInfoText);
                this.topInfoTextTimer--;
            }
        }

        private void DrawInputHistory(Graphics gfx, byte[] inputHistory, AnchorPoint anchorPoint)
        {
            int yMultiplier = 0;
            for (int i = inputHistory.Length - 1; i >= 0; i--)
            {
                byte input = inputHistory.ElementAt(i);

                int x = 20;
                int y = (this.window.Height - 50) - (yMultiplier * 30) + 5;
                InputString inputString = InputToString(input);
                if (y > 15)
                {
                    if (anchorPoint == AnchorPoint.LEFT)
                    {
                        Point directionalStringSize = gfx.MeasureString(this.fonts["arial"], 30, inputString.DirectionalInput);
                        DrawShadedText(gfx, this.fonts["arial"], 30, this.brushes["magenta"], this.brushes["black"], 1, x + 2, y + 2, anchorPoint, inputString.DirectionalInput);
                        DrawShadedText(gfx, this.fonts["arial"], 20, this.brushes["magenta"], this.brushes["black"], 1, x + 2 + directionalStringSize.X, y + 13, anchorPoint, String.Join(' ', inputString.ButtonInputs));
                    }
                    else 
                    {
                        string buttonsString = String.Join(' ', inputString.ButtonInputs);
                        Point buttonsStringSize = gfx.MeasureString(this.fonts["arial"], 20, buttonsString);
                        DrawShadedText(gfx, this.fonts["arial"], 30, this.brushes["magenta"], this.brushes["black"], 1, x + 2 + buttonsStringSize.X, y + 2, anchorPoint, inputString.DirectionalInput);
                        DrawShadedText(gfx, this.fonts["arial"], 20, this.brushes["magenta"], this.brushes["black"], 1, x + 2, y + 13, anchorPoint, buttonsString);
                    }

                }

                yMultiplier++;
            }
        }

        private InputString InputToString(int input)
        {
            InputString inputString = new InputString();

            int directionalInput = input | 0xF0;
            switch (directionalInput)
            {
                case Cheats.INPUT_UP & Cheats.INPUT_LEFT:
                    inputString.DirectionalInput = "⇖";
                    break;
                case Cheats.INPUT_UP & Cheats.INPUT_RIGHT:
                    inputString.DirectionalInput = "⇗";
                    break;
                case Cheats.INPUT_DOWN & Cheats.INPUT_LEFT:
                    inputString.DirectionalInput = "⇙";
                    break;
                case Cheats.INPUT_DOWN & Cheats.INPUT_RIGHT:
                    inputString.DirectionalInput = "⇘";
                    break;
                case Cheats.INPUT_UP:
                    inputString.DirectionalInput = "⇑";
                    break;
                case Cheats.INPUT_DOWN:
                    inputString.DirectionalInput = "⇓";
                    break;
                case Cheats.INPUT_LEFT:
                    inputString.DirectionalInput = "⇐";
                    break;
                case Cheats.INPUT_RIGHT:
                    inputString.DirectionalInput = "⇒";
                    break;
            }

            int buttonlInput = input | 0x0F;
            if ((buttonlInput | Cheats.INPUT_LP) == Cheats.INPUT_LP) inputString.ButtonInputs.Add("A");
            if ((buttonlInput | Cheats.INPUT_LK) == Cheats.INPUT_LK) inputString.ButtonInputs.Add("B");
            if ((buttonlInput | Cheats.INPUT_HP) == Cheats.INPUT_HP) inputString.ButtonInputs.Add("C");
            if ((buttonlInput | Cheats.INPUT_HK) == Cheats.INPUT_HK) inputString.ButtonInputs.Add("D");

            return inputString;
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

        private class InputString
        {
            public string DirectionalInput;
            public List<string> ButtonInputs;

            public InputString()
            {
                DirectionalInput = "";
                ButtonInputs = new List<string>();
            }

            public InputString(string directionalInput, List<string> buttonInputs)
            {
                DirectionalInput = directionalInput;
                ButtonInputs = buttonInputs;
            }
        }

        private void DrawShadedText(Graphics gfx, Font font, float fontSize, IBrush brush, IBrush shade, float shadeSize, float x, float y, AnchorPoint anchor, string text)
        {
            if (anchor == AnchorPoint.RIGHT)
            {
                Point directionalStringSize = gfx.MeasureString(font, fontSize, text);
                x = gfx.Width - x - directionalStringSize.X;
            }
            gfx.DrawText(font, fontSize, shade, x + shadeSize, y, text);
            gfx.DrawText(font, fontSize, shade, x - shadeSize, y, text);
            gfx.DrawText(font, fontSize, shade, x, y + shadeSize, text);
            gfx.DrawText(font, fontSize, shade, x, y - shadeSize, text);
            gfx.DrawText(font, fontSize, brush, x, y, text);

        }
    }
}
