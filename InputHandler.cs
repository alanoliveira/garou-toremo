using System;
using System.Collections.Generic;
using System.Text;

namespace GarouToremo
{
    class InputHandler
    {
        protected const string ADDRESS_P1_INPUT_READ = "Garou.exe+6B9E4"; // mov[Garou.exe+285FD8],ecx -> 89 0D D8 5F 68 00
        protected const string ADDRESS_P2_INPUT_READ = "Garou.exe+6BA42"; // mov [Garou.exe+285FDC],ecx -> 89 0D DC 5F 68 00
        protected const string ADDRESS_P2_INPUT = "Garou.exe+285FDC";
        protected const string ADDRESS_P1_INPUT = "Garou.exe+285FD8";

        protected const int INPUT_RECORD_MAX_SIZE = 60 * 60; // 60 secs
        private const int INPUT_LIST_MEMORY_SIZE = INPUT_RECORD_MAX_SIZE + 2; // 2 bytes for counting

        protected MemoryHandler garouMem;
        private UIntPtr inputListMemory = UIntPtr.Zero;
        private UIntPtr injectedFunction = UIntPtr.Zero;
        private byte[] originalInstruction;

        public InputHandler(MemoryHandler garouMem)
        {
            this.garouMem = garouMem;
            this.inputListMemory = this.garouMem.MemoryAlloc(UIntPtr.Zero, INPUT_LIST_MEMORY_SIZE);
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

        public void InvertControls()
        {
            byte[] p1 = this.garouMem.ReadBytes(ADDRESS_P1_INPUT_READ, 6);
            byte[] p2 = this.garouMem.ReadBytes(ADDRESS_P2_INPUT_READ, 6);

            this.garouMem.WriteBytes(ADDRESS_P1_INPUT_READ, p2);
            this.garouMem.WriteBytes(ADDRESS_P2_INPUT_READ, p1);
        }

        public byte[][] DisableControls()
        {
            byte[] p1 = this.garouMem.ReadBytes(ADDRESS_P1_INPUT_READ, 6);
            byte[] p2 = this.garouMem.ReadBytes(ADDRESS_P2_INPUT_READ, 6);

            byte[] nops = new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 };
            this.garouMem.WriteBytes(ADDRESS_P1_INPUT_READ, nops);
            this.garouMem.WriteBytes(ADDRESS_P2_INPUT_READ, nops);
            this.garouMem.WriteBytes(ADDRESS_P1_INPUT, new byte[] { 0xFF });
            this.garouMem.WriteBytes(ADDRESS_P2_INPUT, new byte[] { 0xFF });

            return new byte[][] { p1, p2 };
        }

        public void ReenableControls(byte[] p1Instruction, byte[] p2Instruction)
        {
            this.garouMem.WriteBytes(ADDRESS_P1_INPUT_READ, p1Instruction);
            this.garouMem.WriteBytes(ADDRESS_P2_INPUT_READ, p2Instruction);
        }

        public byte[] GetRecordedInput()
        {
            UIntPtr inputAddress = UIntPtr.Add(this.inputListMemory, 2); // first two bytes are counter
            return this.garouMem.ReadBytes(inputAddress, INPUT_LIST_MEMORY_SIZE - 2);
        }

        public void StartPlaybackInput(byte[] input)
        {
            garouMem.WriteBytes(this.inputListMemory, input);

            byte[] pXInputRead = garouMem.ReadBytes(ADDRESS_P2_INPUT_READ, 6);
            byte[] inputListAddress = MemoryHandler.PtrToBytes(this.inputListMemory);

            List<byte> instruction = new List<byte>(new byte[]{
                0x50, // push eax 0
                0x53, // push ebx 1
                0x52, // push edx 2
                0x31, 0xC0, // xor eax eax 3
                0x31, 0xDB, // xor ebx ebx 5
                0x31, 0xD2, // xor edx edx 7
                0xB8, inputListAddress[0], inputListAddress[1], inputListAddress[2], inputListAddress[3], // mov eax, COMMAND 9
                0x66, 0x8B, 0x1D, inputListAddress[0], inputListAddress[1], inputListAddress[2], inputListAddress[3], // movzx bl, (word) [COMMAND] 14
                0x8D, 0x8B, inputListAddress[0], inputListAddress[1], inputListAddress[2], inputListAddress[3], // lea edx, [COMMAND+ebx] 21
                0x8A, 0x19, // mov bl, [ecx] 27
                0x80, 0xFB, 0x00, // cmp bl, 0x00 29
                0x74, 0x0C, // JE 0x0C(12) bytes 32
                0x0F, 0x1F, 0x40, 0x00, // nop dword ptr [eax] ; multibyte nop 34
                0x88, 0x1D, pXInputRead[2], pXInputRead[3], pXInputRead[4], pXInputRead[5], // mov [Garou.exe+285FD8], bl 38
                0xFF, 0x00, // inc [eax] 44
                0x5A, // pop edx  45
                0x5B, // pop ebx  46
                0x58, // pop eax  47
            });

            originalInstruction = pXInputRead;
            garouMem.WriteBytes(inputListMemory, new byte[] { 0x02, 0x00 });
            injectedFunction = garouMem.CreateCodeCave(ADDRESS_P2_INPUT_READ, instruction.ToArray(), 6);
        }

        public void StopPlaybackInput()
        {
            garouMem.WriteBytes(ADDRESS_P2_INPUT_READ, originalInstruction);
            FreeInjectedFunction();
        }

        public void StartRecordInput()
        {
            byte[] pXInputRead = garouMem.ReadBytes(ADDRESS_P1_INPUT_READ, 6);
            byte[] inputListAddress = MemoryHandler.PtrToBytes(this.inputListMemory);
            byte[] commandLength = BitConverter.GetBytes(INPUT_RECORD_MAX_SIZE);

            List<byte> instruction = new List<byte>(new byte[]{
                0x50, // push eax 0
                0x53, // push ebx 1
                0x52, // push edx 2
                0x31, 0xC0, // xor eax eax 3
                0x31, 0xDB, // xor ebx ebx 5
                0x31, 0xD2, // xor edx edx 7
                0xB8, inputListAddress[0], inputListAddress[1], inputListAddress[2], inputListAddress[3], // mov eax, COMMAND 9
                0x66, 0x8B, 0x1D, inputListAddress[0], inputListAddress[1], inputListAddress[2], inputListAddress[3], // movzx bl, (word) [COMMAND] 14
                0x8D, 0x93, inputListAddress[0], inputListAddress[1], inputListAddress[2], inputListAddress[3], // lea edx, [COMMAND+ebx] 21
                0x81, 0xFB, commandLength[0], commandLength[1], commandLength[2], commandLength[3], // cmp ebx,MAX_COMMAND_INPUT_LENGTH 27
                0x74, 0x0D, // je 0x0D(47)  33
                0x0F, 0x1F, 0x40, 0x00, // nop dword ptr [eax+00] ; multibyte nop  35
                0x89, 0x0A, // mov [edx],ecx 39
                0xFF, 0x00, // inc [eax] 41
                0xEB, 0x0E, // jmp 0x0E 43
                0x0F, 0x1F, 0x00, // nop dword ptr [eax] ; multibyte nop 45
                0xC7, 0x02, 0x00, 0x00, 0x00, 0x00, // mov [edx], 0x00 48
                0xEB, 0x03, // jmp 0x03 54
                0x0F, 0x1F, 0x00, // nop dword ptr [eax] ; multibyte nop 56
                0x5A, // pop edx  59
                0x5B, // pop ebx  60
                0x58, // pop eax  61
                0x89, 0x0D, pXInputRead[2], pXInputRead[3], pXInputRead[4], pXInputRead[5], // mov [ADDRESS_P2_INPUT],ecx 62
            });

            originalInstruction = pXInputRead;
            garouMem.WriteBytes(inputListMemory, new byte[] { 0x02, 0x00 });
            injectedFunction = garouMem.CreateCodeCave(ADDRESS_P1_INPUT_READ, instruction.ToArray(), 6);
        }

        public void StopRecordInput()
        {
            garouMem.WriteBytes(ADDRESS_P1_INPUT_READ, originalInstruction);
            int currentPosition = garouMem.Read2Byte(this.inputListMemory);
            UIntPtr currentPositionAddress = UIntPtr.Add(this.inputListMemory, currentPosition);
            garouMem.WriteBytes(currentPositionAddress, new byte[] { 0x00 });
            FreeInjectedFunction();
        }

        private void FreeInjectedFunction()
        {
            if (injectedFunction != UIntPtr.Zero)
            {
                garouMem.MemoryFree(this.injectedFunction);
                injectedFunction = UIntPtr.Zero;
            }
        }
    }
}
