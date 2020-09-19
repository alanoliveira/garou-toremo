using System;
using SharpDX.DirectInput;


namespace GarouToremo
{
    class KeyboardHotkey : IHotkeyListenable, IDisposable
    {
        private Keyboard keyboard;
        private Key ResetPositionHotkey;

        private bool resetPositionKeyPressed;
        private bool leftArrowKeyPressed;
        private bool rightArrowKeyPressed;

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
                this.resetPositionKeyPressed = state.IsPressed && state.Key == ResetPositionHotkey;
                this.leftArrowKeyPressed = state.IsPressed && state.Key == Key.Left;
                this.rightArrowKeyPressed = state.IsPressed && state.Key == Key.Right;
            }
        }

        public void SetRestPositionHotkey()
        {
            ResetPositionHotkey = GetPressedKey();
        }

        public bool ResetPositionCenterPressed()
        {
            return resetPositionKeyPressed && !leftArrowKeyPressed && !rightArrowKeyPressed;
        }

        public bool ResetPositionLeftPressed()
        {
            return resetPositionKeyPressed && leftArrowKeyPressed && !rightArrowKeyPressed;
        }

        public bool ResetPositionLRightPressed()
        {
            return resetPositionKeyPressed && !leftArrowKeyPressed && rightArrowKeyPressed;
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
