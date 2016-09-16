using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class SmoothedElementPart
    {
        private readonly Dictionary<XmlResolution, List<GeoPoint>> _lineDictionary = new Dictionary<XmlResolution, List<GeoPoint>>();
        private readonly MapData _mapData;
        private readonly String _originalLinePath;

        internal SmoothedElementPart(OrientedPolygonPart part, MapData mapData)
        {
            _mapData = mapData;
            _originalLinePath = part.Line.Path;

            List<GeoPoint> originalLine = new List<GeoPoint>();

            originalLine.Add(part.Point1.PointList[0]);

            if (part.Orientation == OrientationEnum.DIRECT)
            {
                originalLine.AddRange(part.Line.PointList);
            }
            else
            {
                List<GeoPoint> reversedList = new List<GeoPoint>(part.Line.PointList);
                reversedList.Reverse();
                originalLine.AddRange(reversedList);
            }

            originalLine.Add(part.Point2.PointList[0]);

            if (smooth(originalLine) != 0) throw new Exception();
        }

        // Todo: Another constructor (?) for line elements, which calls the same smooth() method
        // Todo: Another constructor for polygon parts of polygons, which calls another smooth() method.

        private int smooth(List<GeoPoint> originalLine)
        {
            foreach (XmlResolution resolution in _mapData.XmlMapData.resolutionList)
            {
                List<GeoPoint> sampledLine = getSampledLine(originalLine, resolution, 1);
                if (sampledLine == null) return -1;

                


            }

            return 0;
        }

        private List<GeoPoint> getSampledLine(List<GeoPoint> originalLine, XmlResolution resolution, int minMultiplicity)
        {
            List<double> lengthList = new List<double>();
            double length = 0.0;
            int i, pointCount = originalLine.Count;

            if (pointCount < 2)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not sample line '{0}': the line must have at least 2 points", _originalLinePath));
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
                                                                              _originalLinePath, sampledLine.Count - 1, nbSamples));
                return null;
            }

            return sampledLine;
        }
    }
}
