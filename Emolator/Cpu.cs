using System;
using System.Text;

namespace Emolator
{
    public partial class Cpu
    {
        private readonly DataBus dataBus;
        private byte accumulator;
        private byte x;
        private byte y;
        private ushort programCounter = 0x8000;
        private byte stackPointer = 0xff;
        private CpuFlags flags = CpuFlags.Unused;
        private byte waitCycles;

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

        private ushort StackAddress => ToShort(stackPointer, 0x01);

        private static ushort ToShort(byte low, byte high) => (ushort) (low | high << 8);

        private byte ReadByte(ushort address) => dataBus[address];

        private ushort ReadShort(ushort address) => ToShort(ReadByte(address), ReadByte((ushort)(address + 1)));

        private void WriteByte(ushort address, byte value) => dataBus[address] = value;

        /// <summary>
        /// Emulates a 6502 bug that caused the low byte to wrap without incrementing the high byte.
        /// </summary>
        private ushort ReadShortBug(ushort address)
        {
            var a = address;
            var b = (ushort)((a & 0xff00) | (ushort) ((byte) a + 1));
            return ToShort(ReadByte(a), ReadByte(b));
        }

        public void Advance()
        {
            if (waitCycles > 0)
            {
                waitCycles--;
                return;
            }
            var log = new StringBuilder();
            log.Append($"0x{programCounter:X4}");
            var opCode = ReadByte(programCounter);
            var instruction = Instructions[opCode];
            var adressingMode = InstructionAdressingModes[opCode];
            var instructionData = new InstructionData
            {
                ArgumentAddress = GetInstructionArgumentAddress(adressingMode),
                ProgramCounter = programCounter,
                AddressingMode = adressingMode
            };
            var instructionSize = GetInstructionSize(adressingMode);
            for (int i = 0; i < 3; i++)
            {
                if (i <= instructionSize)
                    log.Append($" {ReadByte((ushort) (programCounter + i)):X2}");
                else
                {
                    log.Append("   ");
                }
            }
            programCounter += instructionSize;
            waitCycles = InstructionCycles[opCode];
            if (IsPageCrossed(adressingMode, instructionData.ArgumentAddress))
                waitCycles += InstructionPageCrossCycles[opCode];
            log.Append($" {opCode:X2} {instruction.Method.Name.ToUpper()} {adressingMode} 0x{instructionData.ArgumentAddress:X4}");
            instruction(this, instructionData);
            System.Console.Out.WriteLine(log.ToString());
        }

        private ushort GetInstructionArgumentAddress(AddressingMode mode)
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

        private bool IsPageCrossed(AddressingMode mode, ushort address)
        {
            switch (mode)
            {
                case AddressingMode.AbsoluteX:
                    return AddressPagesDiffer(address, (ushort)(address - x));
                case AddressingMode.AbsoluteY:
                case AddressingMode.IndirectIndexedY:
                    return AddressPagesDiffer(address, (ushort)(address - y));
                default:
                    return false;
            }
        }

        private void SetZero(byte value)
        {
            SetFlag(CpuFlags.Zero, value == 0);
        }

        private void SetNegative(byte value)
        {
            SetFlag(CpuFlags.Negative, (value & 0x80) != 0);
        }

        private void SetZeroNegative(byte value)
        {
            SetZero(value);
            SetNegative(value);
        }

        private void Load(out byte target, ushort address)
        {
            target = ReadByte(address);
            SetZeroNegative(target);
        }

        private void Store(ushort address, byte value)
        {
            WriteByte(address, value);
        }

        private void Transfer(byte from, out byte to)
        {
            to = from;
            SetZeroNegative(to);
        }

        private void Increment(ref byte value)
        {
            value++;
            SetZeroNegative(value);
        }

        private void Decrement(ref byte value)
        {
            value--;
            SetZeroNegative(value);
        }

        private void Branch(bool condition, ushort address)
        {
            if (!condition) return;
            waitCycles++;
            if (AddressPagesDiffer(programCounter, address))
                waitCycles++;
            programCounter = address;
        }

        private static bool AddressPagesDiffer(ushort first, ushort second)
        {
            return (first & 0xff00) != (second & 0xff00);
        }

        private void PushByte(byte value)
        {
            WriteByte(StackAddress, value);
            stackPointer--;
        }

        private void PushShort(ushort value)
        {
            var high = (byte) (value >> 8);
            var low = (byte) value;
            PushByte(high);
            PushByte(low);
        }

        private byte PullByte()
        {
            stackPointer++;
            return ReadByte(StackAddress);
        }

        private ushort PullShort()
        {
            var low = PullByte();
            var high = PullByte();
            return ToShort(low, high);
        }

        private void Jump(ushort address)
        {
            programCounter = address;
        }

        private void RotateRight(ref byte value)
        {
            var c = GetFlag(CpuFlags.Carry) ? 1 : 0;
            SetFlag(CpuFlags.Carry, (value & 1) != 0);
            value = (byte)((value >> 1) | (c << 7));
            SetZeroNegative(value);
        }

        private void RotateLeft(ref byte value)
        {
            var c = GetFlag(CpuFlags.Carry) ? 1 : 0;
            SetFlag(CpuFlags.Carry, ((value >> 7) & 1) != 0);
            value = (byte)((value << 1) | c);
            SetZeroNegative(value);
        }

        private void LogicalShiftLeft(ref byte value)
        {
            SetFlag(CpuFlags.Carry, (value & 1) != 0);
            value >>= 1;
            SetZeroNegative(value);
        }

        private void ArithmeticShiftLeft(ref byte value)
        {
            SetFlag(CpuFlags.Carry, ((value>> 7) & 1) != 0);
            value <<= 1;
            SetZeroNegative(value);
        }

        private void Compare(byte lhs, byte rhs)
        {
            SetZeroNegative((byte) (lhs - rhs));
            SetFlag(CpuFlags.Carry, lhs >= rhs);
        }
    }
}
