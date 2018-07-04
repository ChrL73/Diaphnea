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
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace MapDistortion
{
    class MapDistorsion
    {
        static int n = 8;
        static int m = 8;
        static int w;
        static int h;

        static void Main(string[] args)
        {
            Bitmap image = new Bitmap("DepartementsGabon2.png");

            int w0 = image.Width;
            int h0 = image.Height;
            w = w0 / n;
            h = h0 / m;

            int dw = w / 10;
            int dh = h / 10;

            double[][] A = new double[n + 2][];
            double[][] B = new double[n + 2][];
            double[][] C = new double[n + 2][];
            double[][] D = new double[n + 2][];

            double[][] DX = new double[n + 2][];
            double[][] DY = new double[n + 2][];

            int i, j;
            for (i = 0; i < n + 2; ++i)
            {
                A[i] = new double[m + 2];
                B[i] = new double[m + 2];
                C[i] = new double[m + 2];
                D[i] = new double[m + 2];

                DX[i] = new double[m + 2];
                DY[i] = new double[m + 2];
            }

            double x, y;

            int count = 0;         
            for (i = 0; i < n; ++i)
            {
                for (j = 0; j < m; ++j)
                {
                    string path = String.Format("{0}_{1}.kml", i, j);
                    kml kml = readKml(path);

                    if (kml != null)
                    {
                        int xMin = i * w - dw;
                        if (xMin < 0) xMin = 0;
                        int yMin = j * h - dw;
                        if (yMin < 0) yMin = 0;
                        int xMax = (i + 1) * w + dw;
                        if (xMax > w0) xMax = w0;
                        int yMax = (j + 1) * h + dh;
                        if (yMax > h0) yMax = h0;

                        kmlGroundOverlayLatLonBox box = kml.GroundOverlay.LatLonBox;
                        A[i + 1][j + 1] = (box.east - box.west) / (xMax - xMin);
                        B[i + 1][j + 1] = (box.east * xMin - box.west * xMax) / (xMin - xMax);
                        C[i + 1][j + 1] = (box.south - box.north) / (yMax - yMin);
                        D[i + 1][j + 1] = (box.south * yMin - box.north * yMax) / (yMin - yMax);

                        ++count;
                    }
                }
            }

            double A0 = average(A);
            double B0 = average(B);
            double C0 = average(C);
            double D0 = average(D);

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

                    x = 0.5 * ((double)xMin + (double)xMax);
                    y = 0.5 * ((double)yMin + (double)yMax);

                    if (A[i + 1][j + 1] != 0.0)
                    {
                        DX[i + 1][j + 1] = x - (A[i + 1][j + 1] * x + B[i + 1][j + 1] - B0) / A0;
                        DY[i + 1][j + 1] = y - (C[i + 1][j + 1] * y + D[i + 1][j + 1] - D0) / C0;
                    }

                    string path = String.Format("{0}_{1}.kml", i, j);
                    kml kml = readKml(path);

                    if (kml != null)
                    {
                        kmlGroundOverlayLatLonBox box = kml.GroundOverlay.LatLonBox;

                        double west = box.west;
                        double east = box.east;
                        box.west = 0.5 * (east + west + A0 * (west - east) / A[i + 1][j + 1]);
                        box.east = 0.5 * (east + west + A0 * (east - west) / A[i + 1][j + 1]);

                        double north = box.north;
                        double south = box.south;
                        box.north = 0.5 * (south + north + C0 * (north - south) / C[i + 1][j + 1]);
                        box.south = 0.5 * (south + north + C0 * (south - north) / C[i + 1][j + 1]);

                        path = String.Format("{0}_{1}.kml", i, j);
                        TextWriter writer = new StreamWriter(path);
                        XmlSerializer serializer = new XmlSerializer(typeof(kml));
                        serializer.Serialize(writer, kml);
                        writer.Close();
                    }
                }
            }

            completeArray(DX, count);
            completeArray(DY, count);

            using (StreamWriter file = new System.IO.StreamWriter("coeff.csv"))
            {
                writeFile(file, DX);
                writeFile(file, DY);
                writeFile(file, A);
                writeFile(file, C);
            }

            Bitmap image2 = new Bitmap(w0, h0);
            for (x = 0.0; x < w0; ++x)
            {
                for (y = 0.0; y < h0; ++y)
                {
                    double dx = getInterpolation(DX, x, y);
                    double dy = getInterpolation(DY, x, y);

                    double x0 = x + dx;
                    double y0 = y + dy;

                    Color color;
                    try
                    {
                        color = image.GetPixel((int)x0, (int)y0);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        color = Color.Transparent;
                    }

                    image2.SetPixel((int)x, (int)y, color);
                }
            }

            image2.Save("DepartementsGabon3.png");
        }

        static double getInterpolation(double[][] T, double x, double y)
        {
            double Dx = (double)w;
            double Dy = (double)h;

            double ii = Math.Floor(x / Dx + 0.5);
            int i = (int)ii;
            double jj = Math.Floor(y / Dy + 0.5);
            int j = (int)jj;

            if (i < 0 || i > n || j < 0 || j > m) throw new ArgumentOutOfRangeException();

            double x0 = (ii - 0.5) * Dx;
            double dx = x - x0;
            double y0 = (jj - 0.5) * Dy;
            double dy = y - y0;

            double DTx = T[i + 1][j] - T[i][j];
            double DTy = T[i][j + 1] - T[i][j];
            double DTxy = T[i][j] + T[i + 1][j + 1] - T[i + 1][j] - T[i][j + 1];

            return T[i][j] + DTx * dx / Dx + DTy * dy / Dy + DTxy * dx * dy / (Dx * Dy);
        }

        static double average(double[][] T)
        {
            double S = 0.0;
            int count = 0;

            int i, j;
            for (i = 0; i < n + 2; ++i)
            {
                for (j = 0; j < m + 2; ++j)
                {
                    S += T[i][j];
                    if (T[i][j] != 0.0) ++count;
                }
            }

            return S / (double)count;
        }

        static void completeArray(double[][] T, int count)
        {
            double[][] T1 = new double[n + 2][];

            int i, j;
            for (i = 0; i < n + 2; ++i) T1[i] = new double[m + 2];

            while (count < (n + 2) * (m + 2))
            {
                for (i = 0; i < n + 2; ++i)
                {
                    for (j = 0; j < m + 2; ++j)
                    {
                        if (T[i][j] == 0.0)
                        {
                            int nb = 0;
                            if (j < m + 1 && T[i][j + 1] != 0.0)
                            {
                                T1[i][j] += T[i][j + 1];
                                ++nb;
                            }
                            if (j > 0 && T[i][j - 1] != 0.0)
                            {
                                T1[i][j] += T[i][j - 1];
                                ++nb;
                            }
                            if (i < n + 1 && T[i + 1][j] != 0.0)
                            {
                                T1[i][j] += T[i + 1][j];
                                ++nb;
                            }
                            if (i > 0 && T[i - 1][j] != 0.0)
                            {
                                T1[i][j] += T[i - 1][j];
                                ++nb;
                            }
                            if (nb != 0)
                            {
                                T1[i][j] /= nb;
                                ++count;
                            }
                        }
                    }
                }

                for (i = 0; i < n + 2; ++i)
                {
                    for (j = 0; j < m + 2; ++j)
                    {
                        if (T1[i][j] != 0) T[i][j] = T1[i][j];
                    }
                }
            }
        }

        static void writeFile(StreamWriter file, double[][] T)
        {
            int i, j;
            for (j = 0; j < m + 2; ++j)
            {
                for (i = 0; i < n + 2; ++i)
                {
                    if (T[i][j] != 0) file.Write(T[i][j].ToString());
                    if (i != n + 1) file.Write(";");
                }
                file.WriteLine();
            }
            file.WriteLine();
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
