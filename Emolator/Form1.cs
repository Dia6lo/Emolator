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
}
