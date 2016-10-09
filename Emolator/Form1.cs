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
            var c = RomFactory.CreateRom(a);
            pictureBox1.Image = Image.FromFile("D:\\Downloads\\minimalistic_pixels_dirt_minecraft_pixelation_simple_background_1920x1200_wallpaper_Wallpaper_800x600_www.wall321.com.jpg");
        }
    }

    public class RomFactory
    {
        public static Rom CreateRom(byte[] bytes)
        {
            if ((bytes[7] & 0x0C) == 0x08)
            {
                // TODO: Add ROM size check.
                return new NewNes(bytes);
            }
            if ((bytes[7] & 0x0C) == 0x00)
            {
                //TODO: Add 12-15 emptiness check.
                return new CommonNes(bytes);
            }
            return new ArchaicNes(bytes);
        }
    }

    public class Rom
    {
        private readonly byte[] bytes;

        public Rom(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public byte[] PRG
        {
            get
            {
                // TODO
                return bytes.Skip(16).ToArray();
            }
        }
    }

    public class ArchaicNes: Rom
    {
        public ArchaicNes(byte[] bytes) : base(bytes)
        {
        }
    }

    public class CommonNes : Rom
    {
        public CommonNes(byte[] bytes) : base(bytes)
        {
        }
    }

    public class NewNes : Rom
    {
        public NewNes(byte[] bytes) : base(bytes)
        {
        }
    }
}
