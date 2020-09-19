using System;
using System.Collections.Generic;
using SharpDX.DirectInput;


namespace GarouToremo
{
    class ControllerHotkey : IHotkeyListenable, IDisposable
    {
        private Joystick joystick;
        private JoystickOffset ResetPositionHotkey;
        private JoystickOffset SaveCustomPositionHotkey;
        private List<JoystickOffset> presseKeys = new List<JoystickOffset>();

        private bool leftPressed;
        private bool rightPressed;
        private bool upPressed;
        private bool downPressed;

        public static int DeviceCount()
        {
            var directInput = new DirectInput();
            int joystickCount = directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices).Count;
            int gamepadCount = directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices).Count;

            return joystickCount + gamepadCount;
        }

        public ControllerHotkey(int index)
        {
            var directInput = new DirectInput();
            var joystickGuids = new List<Guid>();

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                joystickGuids.Add(deviceInstance.InstanceGuid);

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                joystickGuids.Add(deviceInstance.InstanceGuid);

            if(joystickGuids.Count < index)
            {
                throw new Exception("Joystick not found");
            }

            var joystick = new Joystick(directInput, joystickGuids[index]);

            joystick.Properties.BufferSize = 128;
            joystick.Acquire();
            this.joystick = joystick;
        }

        public void Update()
        {
            joystick.Poll();
            var datas = joystick.GetBufferedData();
            foreach (var state in datas)
            {
                if (state.Offset == JoystickOffset.PointOfViewControllers0 ||
                    state.Offset == JoystickOffset.PointOfViewControllers1 ||
                    state.Offset == JoystickOffset.PointOfViewControllers2 ||
                    state.Offset == JoystickOffset.PointOfViewControllers3)
                {
                    leftPressed = 27000 == state.Value;
                    rightPressed = 9000 == state.Value;
                    upPressed = 0 == state.Value;
                    downPressed = 18000 == state.Value;
                } 
                else
                {
                    if (state.Value == 128 || state.Value == 65408)
                    {
                        presseKeys.Add(state.Offset);
                    }
                    else
                    {
                        presseKeys.Remove(state.Offset);
                    }
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

        public bool ResetPositionCenterPressed()
        {
            return presseKeys.Contains(ResetPositionHotkey) && !leftPressed && !rightPressed && downPressed;
        }

        public bool ResetPositionLeftPressed()
        {
            return presseKeys.Contains(ResetPositionHotkey) && leftPressed && !rightPressed && !downPressed;
        }

        public bool ResetPositionRightPressed()
        {
            return presseKeys.Contains(ResetPositionHotkey) && !leftPressed && rightPressed && !downPressed;
        }

        public bool ResetPositionCustomPressed()
        {
            return presseKeys.Contains(ResetPositionHotkey) && !leftPressed && !rightPressed && !downPressed;
        }

        public bool SaveCustomPositionPressed()
        {
            return presseKeys.Contains(SaveCustomPositionHotkey);
        }

        private JoystickOffset GetPressedKey()
        {
            joystick.Poll(); // flsuh buffer
            while (true)
            {
                joystick.Poll();
                var datas = joystick.GetBufferedData();
                foreach (var state in datas)
                {
                    if (state.Value == 128 || state.Value == 65408)
                        return state.Offset;
                }
            }
        }

        public void Dispose()
        {
            this.joystick.Dispose();
        }
    }
}
