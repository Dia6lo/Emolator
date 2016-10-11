using System;
using System.Collections.Generic;
using System.Linq;

namespace Emolator
{
    public class Console
    {
        private Cpu cpu;
        private DataBus dataBus;

        // TODO
        private readonly byte[] lowMemory = new byte[0x0100];
        private readonly byte[] medMemory = new byte[0x0400];
        private readonly byte[] highMemory = new byte[0xe300];

        // TODO
        private readonly byte[] program =
        {
            0xa9 ,0xaf ,0x48
        };

        public Console()
        {
            dataBus = new DataBus();
            dataBus.Bind(0x0000, lowMemory);
            dataBus.Bind(0x0200, medMemory);
            dataBus.Bind(0x0600, program);
            dataBus.Bind(0x0700, highMemory);
            cpu = new Cpu(dataBus);
        }

        public void Tick()
        {
            cpu.Advance();
        }
    }

    public class Cpu
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
            var result = -1;
            switch (NextByte())
            {
                case 0x00: // BRK
                    break;
                case 0x48: // PHA
                    result = PushAccumulator();
                    break;
                case 0x65: // ADC
                    result = AddWithCarry(ZeroPage());
                    break;
                case 0x68: // PLA
                    result = PullAccumulator();
                    break;
                case 0x69: // ADC
                    result = AddWithCarry(Immediate());
                    break;
                case 0x85: // STA
                    result = Store(ZeroPage(), accumulator);
                    break;
                case 0x8d: // STA
                    result = Store(Absolute(), accumulator);
                    break;
                case 0x8e: // STX
                    result = Store(Absolute(), x);
                    break;
                case 0xa9: // LDA
                    result = Load(out accumulator, Immediate());
                    break;
                case 0xaa: // TAX
                    result = Transfer(accumulator, out x);
                    break;
                case 0xa2: // LDX
                    result = Load(out x, Immediate());
                    break;
                case 0xca: // DEX
                    result = Decrement(ref x);
                    break;
                case 0xe0: // CPX
                    result = Compare(x, Immediate());
                    break;
                case 0xe8: // INX
                    result = Increment(ref x);
                    break;
                case 0xd0: // BNE
                    result = Branch(!GetFlag(CpuFlags.Zero));
                    break;
            }
            SetFlag(CpuFlags.Zero, result == 0);
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
    }

    [Flags]
    public enum CpuFlags : byte
    {
        Carry            = 1 << 0,
        Zero             = 1 << 1,
        InterruptDisable = 1 << 2,
        DecimalMode      = 1 << 3,
        Break            = 1 << 4,
        Overflow         = 1 << 6,
        Negative         = 1 << 7,
}

    public class DataBus
    {
        // TODO
        private List<Tuple<ushort, byte[]>> bindings = new List<Tuple<ushort, byte[]>>();

        public void Bind(ushort start, byte[] bytes)
        {
            bindings.Add(new Tuple<ushort, byte[]>(start, bytes));
        }

        public byte this[ushort address]
        {
            get
            {
                var memory = bindings.Where(b => b.Item1 <= address)
                    .OrderByDescending(b => b.Item1)
                    .First();
                return memory.Item2[address - memory.Item1];
            }
            set
            {
                var memory = bindings.Where(b => b.Item1 <= address)
                    .OrderByDescending(b => b.Item1)
                    .First();
                memory.Item2[address - memory.Item1] = value;
            }
        }
    }
}
