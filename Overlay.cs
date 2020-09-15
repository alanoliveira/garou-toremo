using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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

            if (e.RecreateResources) return;

            this.fonts["arial"] = gfx.CreateFont("Arial", 12);

        }

        private void DrawWindowGraphics(object sender, DrawGraphicsEventArgs e)
        {
            this.window.PlaceAbove(this.garouWindow);

            Graphics gfx = e.Graphics;


            gfx.ClearScene();

            
            DrawInfoText(gfx);
        }

        private void DrawInfoText(Graphics gfx)
        {
            if(this.infoTextTimer > 0) {
                gfx.DrawText(this.fonts["arial"], this.brushes["green"], 30, window.Height - 30, InfoText);
                this.infoTextTimer--;
            }
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
