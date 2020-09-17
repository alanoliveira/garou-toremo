using System;
using System.Collections.Generic;
using System.Text;

namespace GarouToremo
{
    interface IHotkeyListenable : IDisposable
    {
        public void Update();
        public void SetRestPositionHotkey();
        public bool ResetPositionCenterPressed();
        public bool ResetPositionLeftPressed();
        public bool ResetPositionLRightPressed();
    }
}
