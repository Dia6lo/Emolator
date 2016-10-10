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
        private readonly byte[] lowMemory = new byte[0x0600];

        // TODO
        private readonly byte[] highMemory = new byte[0xe300];

        // TODO
        private readonly byte[] program =
        {
            0xa9, 0xc0, 0xaa, 0xe8, 0x69, 0xc4, 0x00
        };

        public Console()
        {
            dataBus = new DataBus();
            dataBus.Bind(0, lowMemory);
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
        private CpuFlags flags = (CpuFlags) 48; // 00110000

        public Cpu(DataBus dataBus)
        {
            this.dataBus = dataBus;
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
                case 0xa9: // LDA
                    result = LoadAccumulator(programCounter++);
                    break;
                case 0x8d: // STA
                    result = StoreAccumulator(NextShort());
                    break;
                case 0xaa: // TAX
                    result = x = accumulator;
                    break;
                case 0xe8: // INX
                    result = x++;
                    break;
                case 0x69: // ADC
                    result = AddWithCarry(programCounter++);
                    break;
                case 0x00: // BRK
                    break;
            }
            SetFlag(CpuFlags.Zero, result == 0);
        }

        private int LoadAccumulator(ushort address)
        {
            return accumulator = dataBus[address];
        }

        private int StoreAccumulator(ushort address)
        {
            return dataBus[address] = accumulator;
        }

        private int AddWithCarry(ushort address)
        {
            var result = accumulator + dataBus[address];
            SetFlag(CpuFlags.Carry, result > byte.MaxValue);
            return accumulator = (byte) result;
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
                var memory = bindings.Last(b => b.Item1 <= address);
                return memory.Item2[address - memory.Item1];
            }
            set
            {
                var memory = bindings.Last(b => b.Item1 <= address);
                memory.Item2[address - memory.Item1] = value;
            }
        }
    }
}
