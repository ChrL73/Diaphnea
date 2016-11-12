using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ImageSplit
{
    class ImageSplit
    {
        static void Main(string[] args)
        {
            Bitmap image = new Bitmap("DepartementsGabon2.png");

            int w0 = image.Width;
            int h0 = image.Height;

            int n = 8, m = 8;
            int w = w0 / n;
            int h = h0 / m;
            int dw = w / 10;
            int dh = h / 10;

            int i, j;
            for (i = 0; i < n; ++i)
            {
                for (j = 0; j < m; ++j)
                {
                    int xMin = i * w - dw;
                    if (xMin < 0) xMin = 0;
                    int yMin = j * h - dw;
                    if (yMin < 0) yMin = 0;
                    int xMax = (i + 1) * w + dw;
                    if (xMax > w0) xMax = w0;
                    int yMax = (j + 1) * h + dh;
                    if (yMax > h0) yMax = h0;

                    Rectangle rectangle = new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
                    Bitmap subImage = (Bitmap)image.Clone(rectangle, image.PixelFormat);
                    subImage.Save(String.Format("map_{0}_{1}.png", i, j));
                }
            }
        }
    }
}
