using System;

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

        private byte ReadByte(ushort address) => dataBus[address];

        private ushort ReadShort(ushort address) => (ushort)(ReadByte(address) + (ReadByte((ushort)(address + 1)) << 8));

        /// <summary>
        /// Emulates a 6502 bug that caused the low byte to wrap without incrementing the high byte.
        /// </summary>
        private ushort ReadShortBug(ushort address)
        {
            var a = address;
            var b = (ushort)((a * 0xff00) | (ushort) ((byte) a + 1));
            var lo = ReadByte(a);
            var hi = ReadByte(b);
            return (ushort) ((ushort)hi << 8 | lo);
        }

    private byte NextByte() => dataBus[programCounter++];

        private ushort NextShort() => (ushort)(NextByte() + (NextByte() << 8));

        public void Advance()
        {
            var opCode = ReadByte(programCounter);
            var instruction = instructions[opCode];
            var adressingMode = instructionAdressingModes[opCode];
            var instructionData = new InstructionData
            {
                Argument = GetInstructionArgument(adressingMode),
                ProgramCounter = programCounter,
                AddressingMode = adressingMode
            };
            programCounter += GetInstructionSize(adressingMode);
            instruction(this, instructionData);
            System.Console.Out.WriteLine($"{opCode:X} = {instruction.Method.Name} {adressingMode}");
        }

        private ushort GetInstructionArgument(AddressingMode mode)
        {
            var argumentAddress = (ushort) (programCounter + 1);
            switch (mode)
            {
                case AddressingMode.Absolute: return ReadShort(argumentAddress);
                case AddressingMode.AbsoluteX: return (ushort)(ReadShort(argumentAddress) + x);
                case AddressingMode.AbsoluteY: return (ushort)(ReadShort(argumentAddress) + y);
                case AddressingMode.ZeroPage: return ReadByte(argumentAddress);
                case AddressingMode.ZeroPageX: return (ushort)(ReadByte(argumentAddress) + x);
                case AddressingMode.ZeroPageY: return (ushort)(ReadByte(argumentAddress) + y);
                case AddressingMode.Immediate: return argumentAddress;
                case AddressingMode.Relative:
                {
                    var offset = ReadByte(argumentAddress);
                    return offset < 0x80
                        ? (ushort) (programCounter + 2 + offset)
                        : (ushort) (programCounter + 2 + offset - 0x100);
                }
                case AddressingMode.Implied: return 0;
                case AddressingMode.Indirect: return ReadShortBug(ReadShort(argumentAddress));
                case AddressingMode.IndexedIndirectX: return ReadShortBug((ushort)(ReadByte(argumentAddress) + x));
                case AddressingMode.IndirectIndexedY: return (ushort) (ReadShortBug(ReadByte(argumentAddress)) + x);
                case AddressingMode.Accumulator: return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static ushort GetInstructionSize(AddressingMode mode)
        {
            switch (mode)
            {
                case AddressingMode.Absolute: return 3;
                case AddressingMode.AbsoluteX: return 3;
                case AddressingMode.AbsoluteY: return 3;
                case AddressingMode.ZeroPage: return 2;
                case AddressingMode.ZeroPageX: return 2;
                case AddressingMode.ZeroPageY: return 2;
                case AddressingMode.Immediate: return 2;
                case AddressingMode.Relative: return 2;
                case AddressingMode.Implied: return 1;
                case AddressingMode.Indirect: return 3;
                case AddressingMode.IndexedIndirectX: return 2;
                case AddressingMode.IndirectIndexedY: return 2;
                case AddressingMode.Accumulator: return 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
