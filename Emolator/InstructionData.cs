namespace Emolator
{
    public class InstructionData
    {
        public ushort ArgumentAddress { get; set; }
        public ushort ProgramCounter { get; set; }
        public AddressingMode AddressingMode { get; set; }
    }
}