namespace Emolator
{
    public class InstructionData
    {
        public ushort Argument { get; set; }
        public ushort ProgramCounter { get; set; }
        public AddressingMode AddressingMode { get; set; }
    }
}