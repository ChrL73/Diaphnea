using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class ElementLinker
    {
        internal XmlResolution Resolution { get; set; }
        internal XmlProjectionEnum Projection { get; set; }

        private double _yMin = Double.MaxValue;
        private double _yMax = Double.MinValue;
        private const int _n = 5000;
        private const int _digits = 4;
        private readonly double _dy1 = 0.3 * Math.Pow(10.0, -_digits);
        private readonly double _dy2 = 0.6 * Math.Pow(10.0, -_digits);

        private readonly List<LinkerSegment> _segmentList = new List<LinkerSegment>();
        private readonly List<LinkerSegment>[] _segmentArray = new List<LinkerSegment>[_n];

        internal int addPolygon(PolygonMapElement element)
        {
            List<GeoPoint> contour = element.getContourLine(Resolution);

            int i, n = contour.Count;
            for (i = 0; i < n - 1; ++i)
            {
                double x1, y1, x2, y2;
                if (!contour[i].getProjection(Projection, out x1, out y1))
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, "Projection error");
                    return -1;
                }

                if (!contour[i + 1].getProjection(Projection, out x2, out y2))
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, "Projection error");
                    return -1;
                }

                LinkerSegment segment = new LinkerSegment(element, x1, y1, x2, y2);
                _segmentList.Add(segment);

                if (i == 0)
                {
                    if (y1 < _yMin) _yMin = y1;
                    if (y1 > _yMax) _yMax = y1;
                }

                if (y2 < _yMin) _yMin = y2;
                if (y2 > _yMax) _yMax = y2;
            }
            
            return 0;
        }

        internal void classifySegments()
        {
            int i;
            for (i = 0; i < _n; ++i ) _segmentArray[i] = new List<LinkerSegment>();

            foreach(LinkerSegment segment in _segmentList)
            {
                double y1 = Math.Round((double)_n * (segment.Y1 - _yMin) / (_yMax - _yMin), _digits) + _dy1;
                double y2 = Math.Round((double)_n * (segment.Y2 - _yMin) / (_yMax - _yMin), _digits) + _dy1;

                if (y1 <= y2)
                {
                    segment.Y1 = y1;
                    segment.Y2 = y2;
                }
                else
                {
                    segment.Y1 = y2;
                    segment.Y2 = y1;
                    double x = segment.X1;
                    segment.X1 = segment.X2;
                    segment.X2 = x;
                }

                int i1 = getIndex(segment.Y1);
                int i2 = getIndex(segment.Y2);

                for (i = i1; i <= i2; ++i) _segmentArray[i].Add(segment);
            }
        }

        internal List<PolygonMapElement> getLinkedPolygons(PointMapElement pointElement)
        {
            List<PolygonMapElement> polygonList = new List<PolygonMapElement>();

            double x, y;
            if (!pointElement.Point.getProjection(Projection, out x, out y))
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, "Projection error");
                return null;
            }

            y = Math.Round((double)_n * (y - _yMin) / (_yMax - _yMin), _digits) + _dy2;
            int i = getIndex(y);

            Dictionary<PolygonMapElement, int> intersectionCountDictionary = new Dictionary<PolygonMapElement, int>();

            foreach (LinkerSegment segment in _segmentArray[i])
            {
                if (y > segment.Y1 && y < segment.Y2)
                {
                    double xI = segment.X1 + (segment.X2 - segment.X1) * (y - segment.Y1) / (segment.Y2 - segment.Y1);
                    if (xI > x)
                    {
                        if (!intersectionCountDictionary.ContainsKey(segment.Element)) intersectionCountDictionary.Add(segment.Element, 1);
                        else ++intersectionCountDictionary[segment.Element];
                    }
                }
            }

            foreach(KeyValuePair<PolygonMapElement, int> pair in intersectionCountDictionary)
            {
                if (pair.Value % 2 == 1) polygonList.Add(pair.Key);
            }

            return polygonList;
        }

        private int getIndex(double y)
        {
            int index = (int)Math.Floor(y);
            if (index < 0) index = 0;
            if (index >= _n) index = _n - 1;
            return index;
        }
    }
}
