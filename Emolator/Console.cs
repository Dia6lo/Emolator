namespace Emolator
{
    public class Console
    {
        private Cpu cpu;
        private DataBus dataBus;
        
        private readonly byte[] ram = new byte[0x0800];
        
        // TODO
        private readonly byte[] program =
        {
            0xa9 ,0x03 ,0x4c ,0x08 ,0x06 ,0x00 ,0x00 ,0x00 ,0x8d ,0x00 ,0x02
        };

        public Console()
        {
            dataBus = new DataBus();
            dataBus.Bind(0x0000, ram);
            dataBus.Bind(0x0800, ram);
            dataBus.Bind(0x1000, ram);
            dataBus.Bind(0x1800, ram);
            dataBus.Bind(0x2000, program);
            cpu = new Cpu(dataBus);
        }

        public void Tick()
        {
            cpu.Advance();
        }
    }
}