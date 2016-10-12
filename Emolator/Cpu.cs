namespace Emolator
{
    public partial class Cpu
    {
        private readonly DataBus dataBus;
        private byte accumulator;
        private byte x;
        private byte y;
        private ushort programCounter = 0x0600;
        private byte stackPointer = 0xff;
        private CpuFlags flags = (CpuFlags) 48; // 00110000
        private readonly byte[] stack = new byte[0x100];

        public Cpu(DataBus dataBus)
        {
            this.dataBus = dataBus;
            dataBus.Bind(0x0100, stack);
        }

        private bool GetFlag(CpuFlags flag) => flags.HasFlag(flag);

        private void SetFlag(CpuFlags flag, bool value)
        {
            if (value)
                flags |= flag;
            else
                flags &= ~flag;
        }

        private byte NextByte() => dataBus[programCounter++];

        private ushort NextShort() => (ushort)(NextByte() + (NextByte() << 8));

        public void Advance()
        {
            var opCode = NextByte();
            var operation = operations[opCode];
            var adressingMode = adressingModes[opCode];
            System.Console.Out.WriteLine($"{opCode:X} = {operation.Method.Name} {adressingMode}");
        }

        private ushort Absolute() => NextShort();
        private ushort AbsoluteX() => (ushort)(NextShort() + x);
        private ushort AbsoluteY() => (ushort)(NextShort() + y);
        private ushort ZeroPage() => NextByte();
        private ushort ZeroPageX() => (ushort)(NextByte() + x);
        private ushort ZeroPageY() => (ushort)(NextByte() + y);
        private ushort Immediate() => programCounter++;
        private ushort Indirect() => dataBus[Absolute()];
        private ushort IndexedIndirectX() => dataBus[ZeroPageX()];
        private ushort IndirectIndexedY() => (ushort)(dataBus[ZeroPage()] + y);

        private int Load(out byte target, ushort address)
        {
            return target = dataBus[address];
        }

        private int Store(ushort address, byte value)
        {
            return dataBus[address] = value;
        }

        private int AddWithCarry(ushort address)
        {
            var result = accumulator + dataBus[address];
            SetFlag(CpuFlags.Carry, result > byte.MaxValue);
            return accumulator = (byte) result;
        }

        private int Transfer(byte from, out byte to)
        {
            return to = from;
        }

        private int Increment(ref byte value)
        {
            return value++;
        }

        private int Decrement(ref byte value)
        {
            return value--;
        }

        private int Compare(byte value, ushort address)
        {
            var other = dataBus[address];
            SetFlag(CpuFlags.Carry, value >= other);
            return value == other ? 0 : -1;
        }

        private int Branch(bool condition)
        {
            var target = (ushort)(programCounter - (byte.MaxValue - NextByte()));
            if (!condition) return -1;
            programCounter = target;
            return -1;
        }

        private int PushAccumulator()
        {
            stack[stackPointer--] = accumulator;
            return -1;
        }

        private int PullAccumulator()
        {
            accumulator = stack[stackPointer++];
            return -1;
        }

        private int Jump(ushort address)
        {
            programCounter = address;
            return -1;
        }
    }
}
