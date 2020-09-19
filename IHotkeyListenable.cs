using System;
using System.Collections.Generic;
using System.Text;

namespace GarouToremo
{
    interface IHotkeyListenable : IDisposable
    {
        public void Update();
        public void SetRestPositionHotkey();
        public void SetSaveCustomPositionHotkey();
        public void SetToggleRecordHotkey();
        public void SetTogglePlaybackHotkey();
        public bool SaveCustomPositionPressed();
        public bool ResetPositionCenterPressed();
        public bool ResetPositionLeftPressed();
        public bool ResetPositionRightPressed();
        public bool ResetPositionCustomPressed();
        public bool ToggleRecordPressed();
        public bool TogglePlaybackPressed();
    }
}
