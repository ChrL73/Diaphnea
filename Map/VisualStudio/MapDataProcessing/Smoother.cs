using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class Smoother
    {
        static internal List<GeoPoint> smoothLine(List<GeoPoint> originalLine, XmlResolution resolution, String originalLinePath)
        {
            List<GeoPoint> sampledLine = getSampledLine(originalLine, resolution, 1, originalLinePath);
            if (sampledLine == null) return null;

            List<GeoPoint> smoothedLine = getSmoothedLine(sampledLine, resolution, originalLinePath);
            if (smoothedLine == null) return null;

            return smoothedLine;
        }

        static internal List<GeoPoint> smoothPolygon(List<GeoPoint> originalLine, XmlResolution resolution, String originalLinePath)
        {
            throw new NotImplementedException();
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

            if (GeoPoint.distance(sample, lastSample) < sampleLength1 * 0.001)
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
            for (i = 0; i < pointCount; i += sampleRatio)
            {
                Int32 w = Int32.Parse(resolution.smoothingWidth);
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
    }
}
