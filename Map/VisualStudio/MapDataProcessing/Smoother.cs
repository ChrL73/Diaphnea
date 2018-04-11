using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace MapDataProcessing
{
    class Smoother
    {
        static internal List<GeoPoint> smoothLine(List<GeoPoint> originalLine, XmlResolution resolution, String originalLinePath, LineSuperposer superposer, bool below)
        {
            List<GeoPoint> sampledLine = getSampledLine(originalLine, resolution, 1, originalLinePath);
            if (sampledLine == null) return null;

            List<GeoPoint> smoothedLine = getSmoothedLine(sampledLine, resolution, originalLinePath);
            if (smoothedLine == null) return null;

            if (superposer != null)
            {
                if (below) superposer.addReferenceLine(smoothedLine);
                else superposer.superposeLine(smoothedLine);
            }

            return smoothedLine;
        }

        static internal List<GeoPoint> smoothPolygon(List<GeoPoint> originalLine, XmlResolution resolution, String originalLinePath)
        {
            List<GeoPoint> sampledPolygon = getSampledLine(originalLine, resolution, 3, originalLinePath);
            if (sampledPolygon == null) return null;

            List<GeoPoint> smoothedPolygon = getSmoothedPolygon(sampledPolygon, resolution, originalLinePath);
            if (smoothedPolygon == null) return null;

            return smoothedPolygon;
        }

        static internal double getLineLength(List<GeoPoint> line)
        {
            int pointCount = line.Count;
            if (pointCount < 2) return 0.0;

            int i;
            double length = 0.0;

            for (i = 1; i < pointCount; ++i)
            {
                if (i != 0)
                {
                    double dx = line[i].X - line[i - 1].X;
                    double dy = line[i].Y - line[i - 1].Y;
                    double dz = line[i].Z - line[i - 1].Z;
                    length += Math.Sqrt(dx * dx + dy * dy + dz * dz);
                }
            }

            return length;
        }

        static private List<GeoPoint> getSampledLine(List<GeoPoint> originalLine, XmlResolution resolution, int minMultiplicity, String originalLinePath)
        {
            List<double> lengthList = new List<double>();
            double length = 0.0;
            int i, pointCount = originalLine.Count;

            if (pointCount < 2)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not sample line '{0}': the line must have at least 2 points", originalLinePath));
                return null;
            }

            for (i = 0; i < pointCount; ++i)
            {
                if (i != 0)
                {
                    double dx = originalLine[i].X - originalLine[i - 1].X;
                    double dy = originalLine[i].Y - originalLine[i - 1].Y;
                    double dz = originalLine[i].Z - originalLine[i - 1].Z;
                    length += Math.Sqrt(dx * dx + dy * dy + dz * dz);
                }
                lengthList.Add(length);
            }

            List<GeoPoint> sampledLine = new List<GeoPoint>();

            double sampleRatio = Double.Parse(resolution.sampleRatio);
            double sampleLength1 = resolution.sampleLength1;

            double nbSamplesD = sampleRatio * Math.Floor(length / (sampleLength1 * sampleRatio));
            double minNbSamplesD = (double)minMultiplicity * sampleRatio;
            if (nbSamplesD < minNbSamplesD) nbSamplesD = minNbSamplesD;
            sampleLength1 = length / nbSamplesD;
            int nbSamples = (int)nbSamplesD;

            i = 0;
            int j = 0;
            length = sampleLength1;

            GeoPoint sample = new GeoPoint(originalLine[0].Longitude, originalLine[0].Latitude);
            sampledLine.Add(sample);

            while (j < pointCount - 1)
            {
                while (length < lengthList[j + 1])
                {
                    double l = length - lengthList[j];
                    double x = originalLine[j].X;
                    double y = originalLine[j].Y;
                    double z = originalLine[j].Z;
                    double dx = originalLine[j + 1].X - x;
                    double dy = originalLine[j + 1].Y - y;
                    double dz = originalLine[j + 1].Z - z;
                    double dr = lengthList[j + 1] - lengthList[j];
                    sample = new GeoPoint(x + l * dx / dr, y + l * dy / dr, z + l * dz / dr);
                    sampledLine.Add(sample);

                    length += sampleLength1;
                    ++i;
                }

                ++j;
            }

            GeoPoint lastSample = sampledLine[sampledLine.Count - 1];
            sample = new GeoPoint(originalLine[pointCount - 1].Longitude, originalLine[pointCount - 1].Latitude);

            if (GeoPoint.distance(sample, lastSample) < sampleLength1 * 0.01)
            {
                sampledLine.RemoveAt(sampledLine.Count - 1);
            }

            sampledLine.Add(sample);

            if (sampledLine.Count != nbSamples + 1)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Fail to sample line '{0}': Unexpected sample count: {1} instead of {2}",
                                                                              originalLinePath, sampledLine.Count - 1, nbSamples));
                return null;
            }

            return sampledLine;
        }

        static private List<GeoPoint> getSmoothedLine(List<GeoPoint> sampledLine, XmlResolution resolution, String originalLinePath)
        {
            List<GeoPoint> smoothedLine = new List<GeoPoint>();

            Int32 sampleRatio = Int32.Parse(resolution.sampleRatio);
            if ((sampledLine.Count - 1) % sampleRatio != 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Fail to smooth line '{0}': Sample count ({1}) is not divisible by the sample ratio ({2})",
                                                                              originalLinePath, sampledLine.Count - 1, sampleRatio));
                return null;
            }

            int i, pointCount = sampledLine.Count;
            Int32 width = Int32.Parse(resolution.smoothingWidth);
            for (i = 0; i < pointCount; i += sampleRatio)
            {
                Int32 w = width;
                if (w > i) w = i;
                if (w > pointCount - 1 - i) w = pointCount - 1 - i;

                if (w > i)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Fail to smooth line '{0}': Smoothing width ({1}) greater than point index ({2})",
                                                                                  originalLinePath, w, i));
                    return null;
                }

                double d = 0.0;
                double x = 0.0;
                double y = 0.0;
                double z = 0.0;

                int j;
                for (j = i - w; j <= i + w; ++j)
                {
                    double coeff = 1.0;
                    if (w != 0)
                    {
                        double u = (double)(j - i) / (double)(w);
                        coeff = Math.Exp(-5.0 * u * u);
                    }
                    d += coeff;
                    x += coeff * sampledLine[j].X;
                    y += coeff * sampledLine[j].Y;
                    z += coeff * sampledLine[j].Z;
                }

                smoothedLine.Add(new GeoPoint(x / d, y / d, z / d));
            }

            return smoothedLine;
        }

        static private List<GeoPoint> getSmoothedPolygon(List<GeoPoint> sampledPolygon, XmlResolution resolution, String originalLinePath)
        {
            Int32 sampleRatio = Int32.Parse(resolution.sampleRatio);
            if ((sampledPolygon.Count - 1) % sampleRatio != 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Fail to smooth polygon '{0}': Sample count ({1}) is not divisible by the sample ratio ({2})",
                                                                              originalLinePath, sampledPolygon.Count - 1, sampleRatio));
                return null;
            }

            int pointCount = sampledPolygon.Count;
            double x0 = 0.0, y0 = 0.0, z0 = 0.0;
            foreach (GeoPoint p1 in sampledPolygon)
            {
                x0 += p1.X;
                y0 += p1.Y;
                z0 += p1.Z;
            }
            x0 /= (double)pointCount;
            y0 /= (double)pointCount;
            z0 /= (double)pointCount;

            double area0 = getArea(sampledPolygon);

            List<GeoPoint> smoothedPolygon0 = new List<GeoPoint>();

            int i;
            Int32 width = Int32.Parse(resolution.smoothingWidth);
            for (i = 0; i < pointCount; i += sampleRatio)
            {
                double d = 0.0;
                double x = 0.0;
                double y = 0.0;
                double z = 0.0;

                int j;
                for (j = i - width; j <= i + width; ++j)
                {
                    double coeff = 1.0;
                    if (width != 0)
                    {
                        double u = (double)(j - i) / (double)width;
                        coeff = Math.Exp(-5.0 * u * u);
                    }
                    d += coeff;
                    int k = j;
                    while (k < 0) k += pointCount - 1;
                    k %= pointCount - 1;
                    x += coeff * sampledPolygon[k].X;
                    y += coeff * sampledPolygon[k].Y;
                    z += coeff * sampledPolygon[k].Z;
                }

                smoothedPolygon0.Add(new GeoPoint(x / d, y / d, z / d));
            }

            double area1 = getArea(smoothedPolygon0);
            double ratio = Math.Pow(area0 / area1, 0.35);

            List<GeoPoint> smoothedPolygon = new List<GeoPoint>();
            pointCount = smoothedPolygon0.Count;
            for (i = 0; i < pointCount; ++i)
            {
                double x = x0 + ratio * (smoothedPolygon0[i].X - x0);
                double y = y0 + ratio * (smoothedPolygon0[i].Y - y0);
                double z = z0 + ratio * (smoothedPolygon0[i].Z - z0);

                smoothedPolygon.Add(new GeoPoint (x, y, z));
            }

            return smoothedPolygon;
        }

        private static double getArea(List<GeoPoint> pointList)
        {
            double x0 = 0.0, y0 = 0.0, z0 = 0.0;

            foreach (GeoPoint p1 in pointList)
            {
                x0 += p1.X;
                y0 += p1.Y;
                z0 += p1.Z;
            }

            int pointCount = pointList.Count;
            double a = 1.0 / (double)pointCount;
            GeoPoint p0 = new GeoPoint(a * x0, a * y0, a * z0);

            double uX1 = -Math.Sin(p0.Longitude);
            double uX2 = Math.Cos(p0.Longitude);
            double uX3 = 0.0;
            double uY1 = -Math.Cos(p0.Longitude) * Math.Sin(p0.Latitude);
            double uY2 = -Math.Sin(p0.Longitude) * Math.Sin(p0.Latitude);
            double uY3 = Math.Cos(p0.Latitude);

            double area2 = 0.0;

            int i;
            for (i = 0; i < pointCount - 1; ++i)
            {
                GeoPoint p1 = pointList[i];
                GeoPoint p2 = pointList[i + 1];

                double p1X = p1.X * uX1 + p1.Y * uX2 + p1.Z * uX3;
                double p2X = p2.X * uX1 + p2.Y * uX2 + p2.Z * uX3;
                double p1Y = p1.X * uY1 + p1.Y * uY2 + p1.Z * uY3;
                double p2Y = p2.X * uY1 + p2.Y * uY2 + p2.Z * uY3;

                area2 += p1X * p2Y - p1Y * p2X;
            }

            return 0.5 * area2;
        }
    }
}
