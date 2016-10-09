using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Emolator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var a = File.ReadAllBytes("Wheel of Fortune (USA).nes");
            var b = System.Text.Encoding.Default.GetString(a);
            var c = Rom.Create(a);
            pictureBox1.Image = Image.FromFile("D:\\Downloads\\minimalistic_pixels_dirt_minecraft_pixelation_simple_background_1920x1200_wallpaper_Wallpaper_800x600_www.wall321.com.jpg");
        }
    }

    public class Rom
    {
        private readonly byte[] bytes;
        private readonly ArraySegment<byte> prg;

        public Rom(byte[] bytes)
        {
            this.bytes = bytes;
            prg = new ArraySegment<byte>(bytes, 16, 16384 * PrgSize);
        }

        public byte PrgSize => bytes[4];

        public IReadOnlyCollection<byte> Prg => prg;

        public static Rom Create(byte[] bytes)
        {
            switch (bytes[7] & 0x0C)
            {
                case 0x08:
                    // TODO: Add ROM size check.
                    return new NewNesRom(bytes);
                case 0x00:
                    //TODO: Add 12-15 emptiness check.
                    return new CommonNesRom(bytes);
                default:
                    return new ArchaicNesRom(bytes);
            }
        }
    }

    public class ArchaicNesRom: Rom
    {
        public ArchaicNesRom(byte[] bytes) : base(bytes)
        {
        }
    }

    public class CommonNesRom : Rom
    {
        public CommonNesRom(byte[] bytes) : base(bytes)
        {
        }
    }

    public class NewNesRom : Rom
    {
        public NewNesRom(byte[] bytes) : base(bytes)
        {
        }
    }
}
