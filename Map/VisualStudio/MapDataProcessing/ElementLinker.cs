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
        internal double LinePointLinkThreshold { get; set; }   

        private double _yMin = Double.MaxValue;
        private double _yMax = Double.MinValue;
        private const int _n = 5000;
        private const int _digits = 4;
        private readonly double _dy1 = 0.3 * Math.Pow(10.0, -_digits);
        private readonly double _dy2 = 0.6 * Math.Pow(10.0, -_digits);

        private readonly Dictionary<string, MapElement> _elementDictionary = new Dictionary<string, MapElement>();
        private readonly List<LinkerSegment> _segmentList = new List<LinkerSegment>();
        private readonly List<LinkerSegment>[] _segmentArray = new List<LinkerSegment>[_n];

        internal void addElement(string id, MapElement element)
        {
            _elementDictionary.Add(id, element);
        }

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

        internal List<MapElement> getLinked1Elements(MapElement element)
        {
            if (element is PointMapElement)
            {
                List<MapElement> list1 = getLinked1Polygons((PointMapElement)element);
                if (list1 == null) return null;

                List<MapElement> list2 = getLinked1Lines((PointMapElement)element);
                if (list2 == null) return null;

                list1.AddRange(list2);
                return list1;
            }

            if (element is LineMapElement) return getLinked1Polygons((LineMapElement)element);
            if (element is PolygonMapElement) return getLinked1Polygons((PolygonMapElement)element);

            return new List<MapElement>();
        }

        internal List<MapElement> getLinked2Elements(MapElement element)
        {
            if (element is LineMapElement) return getLinked2Lines((LineMapElement)element);
            if (element is PolygonMapElement) return getLinked2Polygons((PolygonMapElement)element);
            return new List<MapElement>();
        }

        private List<MapElement> getLinked1Polygons(PointMapElement pointElement)
        {
            Dictionary<MapElement, int> polygonDictionary = new Dictionary<MapElement, int>();

            double x, y;
            if (!pointElement.Point.getProjection(Projection, out x, out y))
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, "Projection error");
                return null;
            }

            getLinkedPolygons(x, y, polygonDictionary);
            return polygonDictionary.Keys.ToList();
        }

        private List<MapElement> getLinked1Lines(PointMapElement pointElement)
        {
            List<MapElement> lineList = new List<MapElement>();

            foreach (LineMapElement lineElement in _elementDictionary.Values.OfType<LineMapElement>())
            {
                if (lineElement.getDistance(pointElement.Point, Resolution) < LinePointLinkThreshold) lineList.Add(lineElement);
            }

            return lineList;
        }

        private List<MapElement> getLinked1Polygons(LineMapElement lineElement)
        {
            Dictionary<MapElement, int> polygonDictionary = new Dictionary<MapElement, int>();

            List<GeoPoint> points = lineElement.getPoints(Resolution);
            foreach (GeoPoint point in points)
            {
                double x, y;
                if (!point.getProjection(Projection, out x, out y))
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, "Projection error");
                    return null;
                }

                getLinkedPolygons(x, y, polygonDictionary);
            }

            return polygonDictionary.Keys.ToList();
        }

        private List<MapElement> getLinked1Polygons(PolygonMapElement polygonElement)
        {
            List<MapElement> polygonsList = new List<MapElement>();

            foreach (string elementId in polygonElement.CoveredElementList)
            {
                MapElement element;
                if (_elementDictionary.TryGetValue(elementId, out element))
                {
                    if (element is PolygonMapElement) polygonsList.Add((PolygonMapElement)element);
                }
            }

            return polygonsList;
        }

        private void getLinkedPolygons(double x, double y, Dictionary<MapElement, int> polygonDictionary)
        {
            y = Math.Round((double)_n * (y - _yMin) / (_yMax - _yMin), _digits) + _dy2;
            int i = getIndex(y);

            Dictionary<MapElement, int> intersectionCountDictionary = new Dictionary<MapElement, int>();

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

            foreach (KeyValuePair<MapElement, int> pair in intersectionCountDictionary)
            {
                if (pair.Value % 2 == 1 && !polygonDictionary.ContainsKey(pair.Key)) polygonDictionary.Add(pair.Key, 0);
            }
        }

        private List<MapElement> getLinked2Lines(LineMapElement lineElement)
        {
            return new List<MapElement>();
        }

        private List<MapElement> getLinked2Polygons(PolygonMapElement polygonElement)
        {
            return new List<MapElement>();
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
