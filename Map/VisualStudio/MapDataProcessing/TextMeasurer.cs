using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapDataProcessing
{
    class TextMeasurer
    {
        static internal void measure(string text, string fontName, double fontSize, out double width, out double height)
        {
            double w = 0.0;
            double h = 0.0;

            Thread t = new Thread(() =>  
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = fontSize;
                textBlock.FontFamily = new FontFamily(fontName);
                textBlock.Text = text;
                textBlock.Measure(new Size(10000.0, 10000.0));
                w = textBlock.DesiredSize.Width;
                h = textBlock.DesiredSize.Height;
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            width = w;
            height = h;
        }
    }
}
