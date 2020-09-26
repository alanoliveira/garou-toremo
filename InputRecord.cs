namespace GarouToremo
{
    class InputRecord
    {
        public Side PlayerSide;
        public byte[] Inputs;

        public byte[] GetInputCorrectedBySide(Side playerSide)
        {
            byte[] correctedInput = (byte[])Inputs.Clone();

            if (playerSide != this.PlayerSide)
            {
                for (int i = 0; i < correctedInput.Length && correctedInput[i] != 0; i++)
                {
                    if((correctedInput[i] | (Cheats.INPUT_LEFT & Cheats.INPUT_RIGHT)) != Cheats.INPUT_NEUTRAL)
                        correctedInput[i] ^= ~Cheats.INPUT_LEFT ^ ~Cheats.INPUT_RIGHT;
                }
            }

            return correctedInput;
        }

    }
}
