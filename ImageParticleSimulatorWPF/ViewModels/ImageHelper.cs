using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;


public class ImageHelper
{
    public static Color[,] GetPixelColors(BitmapSource bitmap)
    {
        int width = bitmap.PixelWidth;
        int height = bitmap.PixelHeight;

        
        var formattedBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);

        int bytesPerPixel = (formattedBitmap.Format.BitsPerPixel + 7) / 8;
        int stride = width * bytesPerPixel;
        byte[] pixels = new byte[height * stride];

        formattedBitmap.CopyPixels(pixels, stride, 0);

        Color[,] result = new Color[height, width];

        for (int y = 0; y < height; y++)
        {
            int rowStart = y * stride;
            for (int x = 0; x < width; x++)
            {
                int index = rowStart + x * bytesPerPixel;
                byte b = pixels[index];
                byte g = pixels[index + 1];
                byte r = pixels[index + 2];
                byte a = pixels[index + 3];

                result[y, x] = Color.FromArgb(a, r, g, b);
            }
        }

        return result;
    }
}