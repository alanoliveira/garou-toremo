using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Memory;

namespace GarouToremo
{
    class MemoryHandler : Mem
    {
        const uint MEM_COMMIT = 0x00001000;
        const uint MEM_RESERVE = 0x00002000;
        const uint PAGE_EXECUTE_READWRITE = 0x40;

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern UIntPtr VirtualAllocEx(
            IntPtr hProcess,
            UIntPtr lpAddress,
            uint dwSize,
            uint flAllocationType,
            uint flProtect
        );

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool VirtualFreeEx(
            IntPtr hProcess,
            UIntPtr lpAddress,
            UIntPtr dwSize,
            uint dwFreeType
        );

        public UIntPtr MemoryAlloc(UIntPtr lpAddress, int size)
        {
            return VirtualAllocEx(this.pHandle, lpAddress, (uint)size, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
        }

        public void MemoryFree(UIntPtr address)
        {
            VirtualFreeEx(this.pHandle, address, (UIntPtr)0, 0x8000);
        }

        public static byte[] PtrToBytes(UIntPtr ptr)
        {
            return BitConverter.GetBytes(ptr.ToUInt32()).ToArray();
        }

        public byte[] ReadBytes(UIntPtr ptr, long length, string file = "")
        {
            return this.ReadBytes(ptr.ToUInt32().ToString("X4"), length, file);
        }

        public int Read2Byte(UIntPtr ptr, string file = "")
        {
            return this.Read2Byte(ptr.ToUInt32().ToString("X4"), file);
        }
    }
}
