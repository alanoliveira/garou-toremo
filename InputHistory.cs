using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GarouToremo
{
    class InputHistory
    {
        private const int INPUT_QUEUE_SIZE = 100;

        private FixedSizedQueue<byte> history;

        public InputHistory()
        {
            this.history = new FixedSizedQueue<byte>(INPUT_QUEUE_SIZE);
        }

        public void AddInput(byte input)
        {
            if (input != 0 && (history.Count == 0 || history.Last != input))
            {
                this.history.Enqueue(input);
            }
        }

        public byte[] GetEffectiveInputs()
        {
            List<byte> effectiveInputs = new List<byte>();
            for (int i = 0; i < history.Count; i++)
            {
                byte input = history.ElementAt(i);
                if (input == Cheats.INPUT_NEUTRAL)
                {
                    continue;
                }
                byte previousInput = i > 0 ? history.ElementAt(i - 1) : Cheats.INPUT_NEUTRAL;
                int buttonlInput = input | 0x0F;
                int directionalInput = input | 0xF0;
                var effectiveButtonInput = (byte)~(previousInput ^ (buttonlInput & previousInput));
                byte effectiveInput = (byte)(effectiveButtonInput & directionalInput);
                if(effectiveInput != Cheats.INPUT_NEUTRAL)
                    effectiveInputs.Add(effectiveInput);
            }

            return effectiveInputs.ToArray();
        }
    }
}
