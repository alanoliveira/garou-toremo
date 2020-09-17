using Memory;
using System;

namespace GarouToremo
{
    class Cheats
    {
        private const string ADDRESS_P1_INPUT      = "Garou.exe+285FD8";
        private const string ADDRESS_P1_HP         = "Garou.exe+2B648F";
        private const string ADDRESS_P1_POWER      = "Garou.exe+2B64BF";
        private const string ADDRESS_P1_X          = "Garou.exe+2B6420"; // 2 bytes
        private const string ADDRESS_P1_Y          = "Garou.exe+2B6428"; // 2 bytes
        private const string ADDRESS_P2_INPUT      = "Garou.exe+285FDC";
        private const string ADDRESS_P2_HP         = "Garou.exe+2B658F";
        private const string ADDRESS_P2_POWER      = "Garou.exe+2B65BF";
        private const string ADDRESS_P2_X          = "Garou.exe+2B6520"; // 2 bytes
        private const string ADDRESS_P2_Y          = "Garou.exe+2B6528"; // 2 bytes
        private const string ADDRESS_TIMER         = "Garou.exe+2BD491";
        private const string ADDRESS_SCENARIO_X    = "Garou.exe+2B6E20";

        public const byte INPUT_UP      = 0xFE;
        public const byte INPUT_DOWN    = 0xFD;
        public const byte INPUT_LEFT    = 0xFB;
        public const byte INPUT_RIGHT   = 0xF7;
        public const byte INPUT_LP      = 0xEF;
        public const byte INPUT_LK      = 0xDF;
        public const byte INPUT_HP      = 0xBF;
        public const byte INPUT_HK      = 0x7F;
        public const byte INPUT_NEUTRAL = 0xFF;

        public const int POSITION_X_CENTER_P1       = 240;
        public const int POSITION_Y_CENTER_P1       = 0;
        public const int POSITION_X_CENTER_P2       = 400;
        public const int POSITION_Y_CENTER_P2       = 0;
        public const int POSITION_X_CENTER_SCENARIO = 160;

        public const int MAX_TIME = 0x99;
        public const int MAX_HP = 120;
        public const int MAX_POWER = 128;

        public enum Player
        {
            P1 = 1,
            P2 = 2
        }

        private Mem garouMem;

        public Cheats(Mem garouMem)
        {
            this.garouMem = garouMem;
        }

        public void SetTime(int newTime)
        {
            if (newTime < 0)
            {
                newTime = 0;
            }
            else if (newTime > MAX_TIME)
            {
                newTime = MAX_TIME;
            }

            garouMem.WriteBytes(ADDRESS_TIMER, new byte[] { (byte)newTime });
        }

        public void SetHp(Player player, int amount)
        {
            if (amount < 0)
            {
                amount = -1;
            }
            else if (amount > MAX_HP)
            {
                amount = MAX_HP;
            }

            string addr = ADDRESS_P1_HP;
            if (player == Player.P2)
            {
                addr = ADDRESS_P2_HP;
            }

            garouMem.WriteBytes(addr, new byte[] { (byte)amount });
        }

        public void SetPower(Player player, int amount)
        {
            if (amount < 0)
            {
                amount = -1;
            }
            else if (amount > MAX_POWER)
            {
                amount = MAX_POWER;
            }

            string addr = ADDRESS_P1_POWER;
            if (player == Player.P2)
            {
                addr = ADDRESS_P2_POWER;
            }

            garouMem.WriteBytes(addr, new byte[] { (byte)amount });
        }

        public byte GetCurrentInputByte(Player player)
        {
            string addr = ADDRESS_P1_INPUT;
            if (player == Player.P2)
            {
                addr = ADDRESS_P2_INPUT;
            }

            return (byte)garouMem.ReadByte(addr);
        }

        public int[] GetPosition(Player player)
        {
            string addrX = ADDRESS_P1_X;
            string addrY = ADDRESS_P1_Y;
            if (player == Player.P2)
            {
                addrX = ADDRESS_P2_X;
                addrY = ADDRESS_P2_Y;
            }

            int x = garouMem.Read2Byte(addrX);
            int y = garouMem.Read2Byte(addrY);

            return new int[] { x, y };
        }

        public void SetPlayerPosition(Player player, int x, int y)
        {
            string addrX = ADDRESS_P1_X;
            string addrY = ADDRESS_P1_Y;
            if (player == Player.P2)
            {
                addrX = ADDRESS_P2_X;
                addrY = ADDRESS_P2_Y;
            }

            garouMem.WriteBytes(addrX, BitConverter.GetBytes((short)x));
            garouMem.WriteBytes(addrY, BitConverter.GetBytes((short)y));
        }

        public void SetScenarioPosition(int x)
        {
            garouMem.WriteBytes(ADDRESS_SCENARIO_X, BitConverter.GetBytes((short)x));
        }
    }
}
