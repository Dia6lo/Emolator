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
        private ushort programCounter = 0x0600;

        public Cpu(DataBus dataBus)
        {
            this.dataBus = dataBus;
        }

        private byte NextByte() => dataBus[programCounter++];

        private ushort NextShort() => (ushort)(NextByte() + (NextByte() << 8));

        public void Advance()
        {
            switch (NextByte())
            {
                case 0xa9:
                    LoadAccumulator(programCounter++);
                    break;
                case 0x8d:
                    StoreAccumulator(NextShort());
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
