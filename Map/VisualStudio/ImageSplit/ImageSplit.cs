/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ImageSplit
{
    class ImageSplit
    {
        static void Main(string[] args)
        {
            kml kml = readKml("Gabon3.kml");

            Bitmap image = new Bitmap(kml.GroundOverlay.Icon.href);

            int w0 = image.Width;
            int h0 = image.Height;

            int n = 8, m = 8;
            int w = w0 / n;
            int h = h0 / m;
            int dw = w / 10;
            int dh = h / 10;

            kmlGroundOverlayLatLonBox box = kml.GroundOverlay.LatLonBox;
            double a = (box.east - box.west) / w0;
            double b = box.west;
            double c = (box.south - box.north) / h0;
            double d = box.north;

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
                    string name = String.Format("{0}_{1}", i, j);
                    string path = String.Format("map_{0}.png", name);
                    subImage.Save(path);

                    kml kml1 = new kml();
                    kml1.GroundOverlay = new kmlGroundOverlay();
                    kml1.GroundOverlay.name = name;
                    kml1.GroundOverlay.Icon = new kmlGroundOverlayIcon();
                    kml1.GroundOverlay.Icon.href = path;
                    kml1.GroundOverlay.Icon.viewBoundScale = 0.75;
                    kml1.GroundOverlay.LatLonBox = new kmlGroundOverlayLatLonBox();
                    kml1.GroundOverlay.LatLonBox.west = a * xMin + b;
                    kml1.GroundOverlay.LatLonBox.east = a * xMax + b;
                    kml1.GroundOverlay.LatLonBox.north = c * yMin + d;
                    kml1.GroundOverlay.LatLonBox.south = c * yMax + d;

                    TextWriter writer = new StreamWriter(String.Format("{0}.kml", name));
                    XmlSerializer serializer = new XmlSerializer(typeof(kml));
                    serializer.Serialize(writer, kml1);
                    writer.Close();
                }
            }
        }

        static kml readKml(string path)
        {
            kml kml;

            XmlReader reader;
            XmlReaderSettings settings = new XmlReaderSettings();

            try
            {
                reader = XmlReader.Create(path, settings);
            }
            catch (Exception)
            {
                Console.WriteLine("Fail to open file {0}", path);
                return null;
            }

            Console.WriteLine("Reading file " + path + " ...");

            XmlSerializer serializer = new XmlSerializer(typeof(kml));

            try
            {
                kml = (kml)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail to deserialize content of file {0}", path);
                Console.WriteLine(e.Message);
                if (e.InnerException != null) Console.WriteLine(e.InnerException.Message);
                return null;
            }

            reader.Close();

            return kml;
        }
    }
}
