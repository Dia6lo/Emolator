using System;
using System.Windows.Forms;

namespace Emolator
{
    public partial class Cpu
    {
        private static Action<Cpu, InstructionData>[] instructions =
        {
            Brk, Ora, Nop, Nop, Nop, Ora, Asl, Nop, Php, Ora, Asl, Nop, Nop, Ora, Asl, Nop,
            Bpl, Ora, Nop, Nop, Nop, Ora, Asl, Nop, Clc, Ora, Nop, Nop, Nop, Ora, Asl, Nop,
            Jsr, And, Nop, Nop, Bit, And, Rol, Nop, Plp, And, Rol, Nop, Bit, And, Rol, Nop,
            Bmi, And, Nop, Nop, Nop, And, Rol, Nop, Sec, And, Nop, Nop, Nop, And, Rol, Nop,
            Rti, Eor, Nop, Nop, Nop, Eor, Lsr, Nop, Pha, Eor, Lsr, Nop, Jmp, Eor, Lsr, Nop,
            Bvc, Eor, Nop, Nop, Nop, Eor, Lsr, Nop, Cli, Eor, Nop, Nop, Nop, Eor, Lsr, Nop,
            Rts, Adc, Nop, Nop, Nop, Adc, Ror, Nop, Pla, Adc, Ror, Nop, Jmp, Adc, Ror, Nop,
            Bvs, Adc, Nop, Nop, Nop, Adc, Ror, Nop, Sei, Adc, Nop, Nop, Nop, Adc, Ror, Nop,
            Nop, Sta, Nop, Nop, Sty, Sta, Stx, Nop, Dey, Nop, Txa, Nop, Sty, Sta, Stx, Nop,
            Bcc, Sta, Nop, Nop, Sty, Sta, Stx, Nop, Tya, Sta, Txs, Nop, Nop, Sta, Nop, Nop,
            Ldy, Lda, Ldx, Nop, Ldy, Lda, Ldx, Nop, Tay, Lda, Tax, Nop, Ldy, Lda, Ldx, Nop,
            Bcs, Lda, Nop, Nop, Ldy, Lda, Ldx, Nop, Clv, Lda, Tsx, Nop, Ldy, Lda, Ldx, Nop,
            Cpy, Cmp, Nop, Nop, Cpy, Cmp, Dec, Nop, Iny, Cmp, Dex, Nop, Cpy, Cmp, Dec, Nop,
            Bne, Cmp, Nop, Nop, Nop, Cmp, Dec, Nop, Cld, Cmp, Nop, Nop, Nop, Cmp, Dec, Nop,
            Cpx, Sbc, Nop, Nop, Cpx, Sbc, Inc, Nop, Inx, Sbc, Nop, Nop, Cpx, Sbc, Inc, Nop,
            Beq, Sbc, Nop, Nop, Nop, Sbc, Inc, Nop, Sed, Sbc, Nop, Nop, Nop, Sbc, Inc, Nop
        };

        private static AddressingMode[] instructionAdressingModes =
        {
            Imp, Iix, Imp, Imp, Imp, Zpg, Zpg, Imp, Imp, Ime, Acc, Imp, Imp, Abs, Abs, Imp,
            Rel, Iiy, Imp, Imp, Imp, Zpx, Zpx, Imp, Imp, Aby, Imp, Imp, Imp, Abx, Abx, Imp,
            Abs, Iix, Imp, Imp, Zpg, Zpg, Zpg, Imp, Imp, Ime, Acc, Imp, Abs, Abs, Abs, Imp,
            Rel, Iiy, Imp, Imp, Imp, Zpx, Zpx, Imp, Imp, Aby, Imp, Imp, Imp, Abx, Abx, Imp,
            Imp, Iix, Imp, Imp, Imp, Zpg, Zpg, Imp, Imp, Ime, Acc, Imp, Abs, Abs, Abs, Imp,
            Rel, Iiy, Imp, Imp, Imp, Zpx, Zpx, Imp, Imp, Aby, Imp, Imp, Imp, Abx, Abx, Imp,
            Imp, Iix, Imp, Imp, Imp, Zpg, Zpg, Imp, Imp, Ime, Acc, Imp, Ind, Abs, Abs, Imp,
            Rel, Iiy, Imp, Imp, Imp, Zpx, Zpx, Imp, Imp, Aby, Imp, Imp, Imp, Abx, Abx, Imp,
            Imp, Iix, Imp, Imp, Zpg, Zpg, Zpg, Imp, Imp, Imp, Imp, Imp, Abs, Abs, Abs, Imp,
            Rel, Iiy, Imp, Imp, Zpx, Zpx, Zpy, Imp, Imp, Aby, Imp, Imp, Imp, Abx, Imp, Imp,
            Ime, Iix, Ime, Imp, Zpg, Zpg, Zpg, Imp, Imp, Ime, Imp, Imp, Abs, Abs, Abs, Imp,
            Rel, Iiy, Imp, Imp, Zpx, Zpx, Zpy, Imp, Imp, Aby, Imp, Imp, Abx, Abx, Aby, Imp,
            Ime, Iix, Imp, Imp, Zpg, Zpg, Zpg, Imp, Imp, Ime, Imp, Imp, Abs, Abs, Abs, Imp,
            Rel, Iiy, Imp, Imp, Imp, Zpx, Zpx, Imp, Imp, Aby, Imp, Imp, Imp, Abx, Abx, Imp,
            Ime, Iix, Imp, Imp, Zpg, Zpg, Zpg, Imp, Imp, Ime, Imp, Imp, Abs, Abs, Abs, Imp,
            Rel, Iiy, Imp, Imp, Imp, Zpx, Zpx, Imp, Imp, Aby, Imp, Imp, Imp, Abx, Abx, Imp
        };

        private static int[] instructionCycles =
        {
            7, 6, 2, 8, 3, 3, 5, 5, 3, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 4, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 3, 2, 2, 2, 3, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 4, 2, 2, 2, 5, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            2, 6, 2, 6, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4,
            2, 6, 2, 6, 4, 4, 4, 4, 2, 5, 2, 5, 5, 5, 5, 5,
            2, 6, 2, 6, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4,
            2, 5, 2, 5, 4, 4, 4, 4, 2, 4, 2, 4, 4, 4, 4, 4,
            2, 6, 2, 8, 3, 3, 5, 5, 2, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            2, 6, 2, 8, 3, 3, 5, 5, 2, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7
        };

        private static AddressingMode Abs => AddressingMode.Absolute;
        private static AddressingMode Abx => AddressingMode.AbsoluteX;
        private static AddressingMode Aby => AddressingMode.AbsoluteY;
        private static AddressingMode Zpg => AddressingMode.ZeroPage;
        private static AddressingMode Zpx => AddressingMode.ZeroPageX;
        private static AddressingMode Zpy => AddressingMode.ZeroPageY;
        private static AddressingMode Ime => AddressingMode.Immediate;
        private static AddressingMode Rel => AddressingMode.Relative;
        private static AddressingMode Imp => AddressingMode.Implied;
        private static AddressingMode Ind => AddressingMode.Indirect;
        private static AddressingMode Iix => AddressingMode.IndexedIndirectX;
        private static AddressingMode Iiy => AddressingMode.IndirectIndexedY;
        private static AddressingMode Acc => AddressingMode.Accumulator;


        /// <summary>
        /// ADd with Carry
        /// </summary>
        private static void Adc(Cpu cpu, InstructionData data)
        {
            var a = cpu.accumulator;
            var b = cpu.ReadByte(data.ArgumentAddress);
            var c = cpu.GetFlag(CpuFlags.Carry) ? 1 : 0;
            var result = a + b + c;
            cpu.accumulator = (byte) result;
            cpu.SetZeroNegative(cpu.accumulator);
            cpu.SetFlag(CpuFlags.Carry, result > 0xff);
            cpu.SetFlag(CpuFlags.Overflow, (((a ^ b) & 0x80) == 0) && (((a ^ cpu.accumulator) & 0x80) != 0));
        }

        /// <summary>
        /// bitwise AND with accumulator
        /// </summary>
        private static void And(Cpu cpu, InstructionData data)
        {
            cpu.accumulator &= cpu.ReadByte(data.ArgumentAddress);
            cpu.SetZeroNegative(cpu.accumulator);
        }

        /// <summary>
        /// Arithmetic Shift Left
        /// </summary>
        private static void Asl(Cpu cpu, InstructionData data)
        {
            if (data.AddressingMode == AddressingMode.Accumulator)
            {
                cpu.ArithmeticShiftLeft(ref cpu.accumulator);
            }
            else
            {
                var value = cpu.ReadByte(data.ArgumentAddress);
                cpu.ArithmeticShiftLeft(ref value);
                cpu.WriteByte(data.ArgumentAddress, value);
            }
        }

        /// <summary>
        /// test BITs
        /// </summary>
        private static void Bit(Cpu cpu, InstructionData data)
        {
            var value = cpu.ReadByte(data.ArgumentAddress);
            cpu.SetFlag(CpuFlags.Overflow, ((value >> 6) & 1) != 0);
            cpu.SetZero((byte) (value & cpu.accumulator));
            cpu.SetNegative(value);
        }

        /// <summary>
        /// Branch on PLus
        /// </summary>
        private static void Bpl(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Branch on MInus
        /// </summary>
        private static void Bmi(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Branch on oVerflow Clear
        /// </summary>
        private static void Bvc(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Branch on oVerflow Set
        /// </summary>
        private static void Bvs(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Branch on Carry Clear
        /// </summary>
        private static void Bcc(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Branch on Carry Set
        /// </summary>
        private static void Bcs(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Branch on Not Equal
        /// </summary>
        private static void Bne(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Branch on EQual
        /// </summary>
        private static void Beq(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// BReaK
        /// </summary>
        private static void Brk(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// CoMPare accumulator
        /// </summary>
        private static void Cmp(Cpu cpu, InstructionData data)
        {
            cpu.Compare(cpu.accumulator, cpu.ReadByte(data.ArgumentAddress));
        }

        /// <summary>
        /// ComPare X register
        /// </summary>
        private static void Cpx(Cpu cpu, InstructionData data)
        {
            cpu.Compare(cpu.x, cpu.ReadByte(data.ArgumentAddress));
        }

        /// <summary>
        /// ComPare Y register
        /// </summary>
        private static void Cpy(Cpu cpu, InstructionData data)
        {
            cpu.Compare(cpu.y, cpu.ReadByte(data.ArgumentAddress));
        }

        /// <summary>
        /// DECrement memory
        /// </summary>
        private static void Dec(Cpu cpu, InstructionData data)
        {
            var value = cpu.ReadByte(data.ArgumentAddress);
            cpu.Decrement(ref value);
            cpu.WriteByte(data.ArgumentAddress, value);
        }

        /// <summary>
        /// bitwise Exclusive OR
        /// </summary>
        private static void Eor(Cpu cpu, InstructionData data)
        {
            cpu.accumulator ^= cpu.ReadByte(data.ArgumentAddress);
            cpu.SetZeroNegative(cpu.accumulator);
        }

        /// <summary>
        /// CLear Carry
        /// </summary>
        private static void Clc(Cpu cpu, InstructionData data)
        {
            cpu.SetFlag(CpuFlags.Carry, false);
        }

        /// <summary>
        /// SEt Carry
        /// </summary>
        private static void Sec(Cpu cpu, InstructionData data)
        {
            cpu.SetFlag(CpuFlags.Carry, true);
        }

        /// <summary>
        /// CLear Interrupt
        /// </summary>
        private static void Cli(Cpu cpu, InstructionData data)
        {
            cpu.SetFlag(CpuFlags.InterruptDisable, false);
        }

        /// <summary>
        /// SEt Interrupt
        /// </summary>
        private static void Sei(Cpu cpu, InstructionData data)
        {
            cpu.SetFlag(CpuFlags.InterruptDisable, true);
        }

        /// <summary>
        /// CLear oVerflow
        /// </summary>
        private static void Clv(Cpu cpu, InstructionData data)
        {
            cpu.SetFlag(CpuFlags.Overflow, false);
        }

        /// <summary>
        /// CLear Decimal
        /// </summary>
        private static void Cld(Cpu cpu, InstructionData data)
        {
            cpu.SetFlag(CpuFlags.DecimalMode, false);
        }

        /// <summary>
        /// SEt Decimal
        /// </summary>
        private static void Sed(Cpu cpu, InstructionData data)
        {
            cpu.SetFlag(CpuFlags.DecimalMode, true);
        }

        /// <summary>
        /// INCrement memory
        /// </summary>
        private static void Inc(Cpu cpu, InstructionData data)
        {
            var value = cpu.ReadByte(data.ArgumentAddress);
            cpu.Increment(ref value);
            cpu.WriteByte(data.ArgumentAddress, value);
        }

        /// <summary>
        /// JuMP
        /// </summary>
        private static void Jmp(Cpu cpu, InstructionData data)
        {
            cpu.Jump(data.ArgumentAddress);
        }

        /// <summary>
        /// Jump to SubRoutine
        /// </summary>
        private static void Jsr(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// LoaD Accumulator
        /// </summary>
        private static void Lda(Cpu cpu, InstructionData data)
        {
            cpu.Load(out cpu.accumulator, data.ArgumentAddress);
        }

        /// <summary>
        /// LoaD X registerSEt Decimal
        /// </summary>
        private static void Ldx(Cpu cpu, InstructionData data)
        {
            cpu.Load(out cpu.x, data.ArgumentAddress);
        }

        /// <summary>
        /// LoaD Y register
        /// </summary>
        private static void Ldy(Cpu cpu, InstructionData data)
        {
            cpu.Load(out cpu.y, data.ArgumentAddress);
        }

        /// <summary>
        /// Logical Shift Right
        /// </summary>
        private static void Lsr(Cpu cpu, InstructionData data)
        {
            if (data.AddressingMode == AddressingMode.Accumulator)
            {
                cpu.LogicalShiftLeft(ref cpu.accumulator);
            }
            else
            {
                var value = cpu.ReadByte(data.ArgumentAddress);
                cpu.LogicalShiftLeft(ref value);
                cpu.WriteByte(data.ArgumentAddress, value);
            }
        }

        /// <summary>
        /// No OPeration
        /// </summary>
        private static void Nop(Cpu cpu, InstructionData data)
        {
        }

        /// <summary>
        /// bitwise OR with Accumulator
        /// </summary>
        private static void Ora(Cpu cpu, InstructionData data)
        {
            cpu.accumulator = (byte) (cpu.accumulator | cpu.ReadByte(data.ArgumentAddress));
            cpu.SetZeroNegative(cpu.accumulator);
        }

        /// <summary>
        /// Transfer A to X
        /// </summary>
        private static void Tax(Cpu cpu, InstructionData data)
        {
            cpu.Transfer(cpu.accumulator, out cpu.x);
        }

        /// <summary>
        /// Transfer X to A
        /// </summary>
        private static void Txa(Cpu cpu, InstructionData data)
        {
            cpu.Transfer(cpu.x, out cpu.accumulator);
        }

        /// <summary>
        /// DEcrement X
        /// </summary>
        private static void Dex(Cpu cpu, InstructionData data)
        {
            cpu.Decrement(ref cpu.x);
        }

        /// <summary>
        /// INcrement X
        /// </summary>
        private static void Inx(Cpu cpu, InstructionData data)
        {
            cpu.Increment(ref cpu.x);
        }

        /// <summary>
        /// Transfer A to Y
        /// </summary>
        private static void Tay(Cpu cpu, InstructionData data)
        {
            cpu.Transfer(cpu.accumulator, out cpu.y);
        }

        /// <summary>
        /// Transfer Y to A
        /// </summary>
        private static void Tya(Cpu cpu, InstructionData data)
        {
            cpu.Transfer(cpu.y, out cpu.accumulator);
        }

        /// <summary>
        /// DEcrement Y
        /// </summary>
        private static void Dey(Cpu cpu, InstructionData data)
        {
            cpu.Decrement(ref cpu.y);
        }

        /// <summary>
        /// INcrement Y
        /// </summary>
        private static void Iny(Cpu cpu, InstructionData data)
        {
            cpu.Increment(ref cpu.y);
        }

        /// <summary>
        /// ROtate Left
        /// </summary>
        private static void Rol(Cpu cpu, InstructionData data)
        {
            if (data.AddressingMode == AddressingMode.Accumulator)
            {
                cpu.RotateLeft(ref cpu.accumulator);
            }
            else
            {
                var value = cpu.ReadByte(data.ArgumentAddress);
                cpu.RotateLeft(ref value);
                cpu.WriteByte(data.ArgumentAddress, value);
            }
        }

        /// <summary>
        /// ROtate Right
        /// </summary>
        private static void Ror(Cpu cpu, InstructionData data)
        {
            if (data.AddressingMode == AddressingMode.Accumulator)
            {
                cpu.RotateRight(ref cpu.accumulator);
            }
            else
            {
                var value = cpu.ReadByte(data.ArgumentAddress);
                cpu.RotateRight(ref value);
                cpu.WriteByte(data.ArgumentAddress, value);
            }
        }

        /// <summary>
        /// ReTurn from Interrupt
        /// </summary>
        private static void Rti(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ReTurn from Subroutine
        /// </summary>
        private static void Rts(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SuBtract with Carry
        /// </summary>
        private static void Sbc(Cpu cpu, InstructionData data)
        {
            var a = cpu.accumulator;
            var b = cpu.ReadByte(data.ArgumentAddress);
            var result = a - b;
            if (cpu.GetFlag(CpuFlags.Carry))
                result -= 1;
            cpu.accumulator = (byte) result;
            cpu.SetZeroNegative(cpu.accumulator);
            cpu.SetFlag(CpuFlags.Carry, result >= 0);
            cpu.SetFlag(CpuFlags.Overflow, (((a ^ b) & 0x80) != 0) && (((a ^ cpu.accumulator) & 0x80) != 0));

        }

        /// <summary>
        /// STore Accumulator
        /// </summary>
        private static void Sta(Cpu cpu, InstructionData data)
        {
            cpu.Store(data.ArgumentAddress, cpu.accumulator);
        }

        /// <summary>
        /// Transfer X to Stack ptr
        /// </summary>
        private static void Txs(Cpu cpu, InstructionData data)
        {
            cpu.stackPointer = cpu.x;
        }

        /// <summary>
        /// Transfer Stack ptr to X
        /// </summary>
        private static void Tsx(Cpu cpu, InstructionData data)
        {
            cpu.x = cpu.stackPointer;
            cpu.SetZeroNegative(cpu.x);
        }

        /// <summary>
        /// PusH Accumulator
        /// </summary>
        private static void Pha(Cpu cpu, InstructionData data)
        {
            cpu.PushByte(cpu.accumulator);
        }

        /// <summary>
        /// PuLl Accumulator
        /// </summary>
        private static void Pla(Cpu cpu, InstructionData data)
        {
            cpu.accumulator = cpu.PullByte();
            cpu.SetZeroNegative(cpu.accumulator);
        }

        /// <summary>
        /// PusH Processor status
        /// </summary>
        private static void Php(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// PuLl Processor status
        /// </summary>
        private static void Plp(Cpu cpu, InstructionData data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// STore X register
        /// </summary>
        private static void Stx(Cpu cpu, InstructionData data)
        {
            cpu.Store(data.ArgumentAddress, cpu.x);
        }

        /// <summary>
        /// STore Y register
        /// </summary>
        private static void Sty(Cpu cpu, InstructionData data)
        {
            cpu.Store(data.ArgumentAddress, cpu.y);
        }
    }
}
