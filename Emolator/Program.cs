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
            console.LoadRom(c);
            while (true)
            {
                console.Tick();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
