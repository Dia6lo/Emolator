using System;

namespace Emolator
{
    public class Console
    {
        private Cpu cpu = new Cpu();
        private short[] memory = new short[0x10000];

        public Console()
        {

        }
    }

    public class Cpu
    {
        private byte accumulator;
        private short programCounter;

        // TODO
        private readonly byte[] memory = new byte[0xffff];

        // TODO
        private readonly byte[] program = {
            0xa9,
            0x01,
            0x8d,
            0x00,
            0x02,
            0xa9,
            0x05,
            0x8d,
            0x01,
            0x02,
            0xa9,
            0x08,
            0x8d,
            0x02,
            0x02
        };

        private byte NextByte() => program[programCounter++];

        private ushort NextShort() => (ushort)(NextByte() + (NextByte() << 8));

        public void Advance()
        {
            switch (NextByte())
            {
                case 0xa9:
                    LoadAccumulator();
                    break;
                case 0x8d:
                    StoreAccumulator();
                    break;
            }
        }

        private void LoadAccumulator()
        {
            accumulator = NextByte();
        }

        private void StoreAccumulator()
        {
            var nextShort = NextShort();
            memory[nextShort] = accumulator;
        }
    }
}
