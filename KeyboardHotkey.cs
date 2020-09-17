using System;
using SharpDX.DirectInput;


namespace GarouToremo
{
    class KeyboardHotkey : IHotkeyListenable, IDisposable
    {
        private Keyboard keyboard;
        private Key ResetPositionHotkey;

        private bool ResetPositionKeyPressed;
        private bool LeftArrowKeyPressed;
        private bool RightArrowKeyPressed;

        public KeyboardHotkey()
        {
            var directInput = new DirectInput();
            var keyboard = new Keyboard(directInput);

            keyboard.Properties.BufferSize = 128;
            keyboard.Acquire();
            this.keyboard = keyboard;
        }

        public void Update()
        {
            keyboard.Poll();
            var datas = keyboard.GetBufferedData();
            foreach (var state in datas)
            {
                this.ResetPositionKeyPressed = state.IsPressed && state.Key == ResetPositionHotkey;
                this.LeftArrowKeyPressed = state.IsPressed && state.Key == Key.Left;
                this.RightArrowKeyPressed = state.IsPressed && state.Key == Key.Right;
            }
        }

        public void SetRestPositionHotkey()
        {
            ResetPositionHotkey = GetPressedKey();
        }

        public bool ResetPositionCenterPressed()
        {
            return ResetPositionKeyPressed && !LeftArrowKeyPressed && !RightArrowKeyPressed;
        }

        public bool ResetPositionLeftPressed()
        {
            return ResetPositionKeyPressed && LeftArrowKeyPressed && !RightArrowKeyPressed;
        }

        public bool ResetPositionLRightPressed()
        {
            return ResetPositionKeyPressed && !LeftArrowKeyPressed && RightArrowKeyPressed;
        }

        private Key GetPressedKey()
        {
            while (true)
            {
                keyboard.Poll();
                var datas = keyboard.GetBufferedData();
                foreach (var state in datas)
                {
                    if (state.IsPressed)
                        return state.Key;
                }
            }
        }

        public void Dispose()
        {
            this.keyboard.Dispose();
        }
    }
}
