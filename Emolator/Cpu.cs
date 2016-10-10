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
        private byte flags = 48; // 00110000

        public Cpu(DataBus dataBus)
        {
            this.dataBus = dataBus;
        }

        private bool CarryFlag { get { return GetFlag(0); } set { SetFlag(0, value); } }
        private bool ZeroFlag { get { return GetFlag(1); } set { SetFlag(1, value); } }
        private bool InterruptDisableFlag { get { return GetFlag(2); } set { SetFlag(2, value); } }
        private bool DecimalModeFlag { get { return GetFlag(3); } set { SetFlag(3, value); } }
        private bool BreakCommandFlag { get { return GetFlag(4); } set { SetFlag(4, value); } }
        private bool OverflowFlag { get { return GetFlag(6); } set { SetFlag(6, value); } }
        private bool NegativeFlag { get { return GetFlag(7); } set { SetFlag(7, value); } }

        private bool GetFlag(int index)
        {
            return (flags & (1 << index)) != 0;
        }

        private void SetFlag(int index, bool value)
        {
            if (value)
                flags |= (byte)(1 << index);
            else
                flags &= (byte)~(1 << index);
        }

        private byte NextByte() => dataBus[programCounter++];

        private ushort NextShort() => (ushort)(NextByte() + (NextByte() << 8));

        public void Advance()
        {
            switch (NextByte())
            {
                case 0xa9: // LDA
                    LoadAccumulator(programCounter++);
                    break;
                case 0x8d: // STA
                    StoreAccumulator(NextShort());
                    break;
                case 0xaa: // TAX
                    x = accumulator;
                    break;
                case 0xe8: // INX
                    x++;
                    break;
                case 0x69: // ADC
                    AddWithCarry(programCounter++);
                    break;
                case 0x00: // BRK
                    break;
            }
        }

        private void LoadAccumulator(ushort address)
        {
            accumulator = dataBus[address];
        }

        private void StoreAccumulator(ushort address)
        {
            dataBus[address] = accumulator;
        }

        private void AddWithCarry(ushort address)
        {
            var result = accumulator + dataBus[address];
            accumulator = (byte) result;
            CarryFlag = result > byte.MaxValue;
        }
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
