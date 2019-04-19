using System.Drawing;
using System.IO;

namespace HotPot.Utility.Extension
{
    public static class BitmapExtension
    {
        public static byte[] ImageToByte(this Image img)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static Bitmap CropImage(this Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }


        public static Bitmap CropImage(this byte[] imageData, Rectangle section)
        {
            var source = new Bitmap(new MemoryStream(imageData));
            if (section.Location.X == 0 && section.Location.Y == 0 && section.Size.Height == 0 && section.Size.Width == 0)
            {
                return null;
            }
            return CropImage(source, section);
        }
    }
}
