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
            0xa9 ,0x03 ,0x4c ,0x08 ,0x06 ,0x00 ,0x00 ,0x00 ,0x8d ,0x00 ,0x02
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
}