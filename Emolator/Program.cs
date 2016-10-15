using System;
using System.IO;
using System.Windows.Forms;

namespace Emolator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var console = new Console();
            var a = File.ReadAllBytes("nestest.nes");
            var b = System.Text.Encoding.Default.GetString(a);
            var c = Rom.Create(a);
            var log = File.ReadAllLines("nestest.log.txt");
            var i = 0;
            console.LoadRom(c);
            while (true)
            {
                var line = log[i];
                var address = line.Substring(0, 4);
                var bytes = line.Substring(6, 9).Split(new []{ " " }, StringSplitOptions.RemoveEmptyEntries);
                var accumulator = line.Substring(50, 2);
                var x = line.Substring(55, 2);
                var y = line.Substring(60, 2);
                var flags = line.Substring(65, 2);
                var stack = line.Substring(71, 2);
                if (!console.cpu.Assert(address, bytes, accumulator, x, y, flags, stack))
                    throw new Exception();
                if (console.cpu.Advance())
                    i++;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
