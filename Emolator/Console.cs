using System.Linq;

namespace Emolator
{
    public class Console
    {
        private Cpu cpu;
        private DataBus dataBus;

        private readonly byte[] ram = new byte[0x0800];

        // TODO
        private readonly byte[] someMemory = new byte[0x6000];

        public Console()
        {
            dataBus = new DataBus();
            dataBus.Bind(0x0000, ram);
            dataBus.Bind(0x0800, ram);
            dataBus.Bind(0x1000, ram);
            dataBus.Bind(0x1800, ram);
            dataBus.Bind(0x2000, someMemory);
            cpu = new Cpu(dataBus);
        }

        public void LoadRom(Rom rom)
        {
            dataBus.Bind(0x8000, rom.Prg.ToArray());
            dataBus.Bind(0xC000, rom.Prg.ToArray());
        }

        public void Tick()
        {
            cpu.Advance();
        }
    }
}