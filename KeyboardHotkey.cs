using System;
using System.Collections.Generic;
using SharpDX.DirectInput;


namespace GarouToremo
{
    class KeyboardHotkey : IHotkeyListenable, IDisposable
    {
        private Keyboard keyboard;
        private Key ResetPositionHotkey;
        private Key SaveCustomPositionHotkey;
        private Key ToggleRecordHotkey;
        private Key TogglePlaybackHotkey;
        private List<Key> presseKeys = new List<Key>();

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
                if(state.IsPressed)
                {
                    presseKeys.Add(state.Key);
                }
                else
                {
                    presseKeys.Remove(state.Key);
                }
            }
        }

        public void SetRestPositionHotkey()
        {
            ResetPositionHotkey = GetPressedKey();
        }

        public void SetSaveCustomPositionHotkey()
        {
            SaveCustomPositionHotkey = GetPressedKey();
        }

        public void SetToggleRecordHotkey()
        {
            ToggleRecordHotkey = GetPressedKey();
        }

        public bool ToggleRecordPressed()
        {
            return presseKeys.Contains(ToggleRecordHotkey);
        }

        public void SetTogglePlaybackHotkey()
        {
            TogglePlaybackHotkey = GetPressedKey();
        }

        public bool TogglePlaybackPressed()
        {
            return presseKeys.Contains(TogglePlaybackHotkey);
        }

        public bool ResetPositionCenterPressed()
        {
            return presseKeys.Contains(ResetPositionHotkey) && !presseKeys.Contains(Key.Left) && !presseKeys.Contains(Key.Right) && presseKeys.Contains(Key.Down);
        }

        public bool ResetPositionLeftPressed()
        {
            return presseKeys.Contains(ResetPositionHotkey) && presseKeys.Contains(Key.Left) && !presseKeys.Contains(Key.Right) && !presseKeys.Contains(Key.Down);
        }

        public bool ResetPositionRightPressed()
        {
            return presseKeys.Contains(ResetPositionHotkey) && !presseKeys.Contains(Key.Left) && presseKeys.Contains(Key.Right) && !presseKeys.Contains(Key.Down);
        }

        public bool ResetPositionCustomPressed()
        {
            return presseKeys.Contains(ResetPositionHotkey) && !presseKeys.Contains(Key.Left) && !presseKeys.Contains(Key.Right) && !presseKeys.Contains(Key.Down);
        }

        public bool SaveCustomPositionPressed()
        {
            return presseKeys.Contains(SaveCustomPositionHotkey);
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
