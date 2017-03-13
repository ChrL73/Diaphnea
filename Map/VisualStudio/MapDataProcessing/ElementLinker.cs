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
        private readonly Dictionary<LineMapElement, Dictionary<LineMapElement, int>> _attachmentDictionary = new Dictionary<LineMapElement, Dictionary<LineMapElement, int>>();
        private readonly Dictionary<PolygonMapElement, Dictionary<PolygonMapElement, int>> _neighborPolygonDictionary = new Dictionary<PolygonMapElement, Dictionary<PolygonMapElement, int>>();

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

        internal void addAttachment(LineMapElement line1, LineMapElement line2)
        {
            if (!_attachmentDictionary.ContainsKey(line1)) _attachmentDictionary.Add(line1, new Dictionary<LineMapElement, int>());
            if (!_attachmentDictionary[line1].ContainsKey(line2)) _attachmentDictionary[line1].Add(line2, 0);
        }

        internal void addNeighbor(PolygonMapElement polygon1, PolygonMapElement polygon2)
        {
            if (!_neighborPolygonDictionary.ContainsKey(polygon1)) _neighborPolygonDictionary.Add(polygon1, new Dictionary<PolygonMapElement, int>());
            if (!_neighborPolygonDictionary[polygon1].ContainsKey(polygon2)) _neighborPolygonDictionary[polygon1].Add(polygon2, 0);
        }

        internal Dictionary<MapElement, int> getLinked1Elements(MapElement element)
        {
            Dictionary<MapElement, int> Linked1Elements = new Dictionary<MapElement, int>();

            if (element is PointMapElement)
            {
                if (getLinked1Polygons((PointMapElement)element, Linked1Elements) != 0) return null;
                if (getLinked1Lines((PointMapElement)element, Linked1Elements) != 0) return null;
            }
            else if (element is LineMapElement)
            {
                if (getLinked1Polygons((LineMapElement)element, Linked1Elements) != 0) return null;
                if (getLinked1Lines((LineMapElement)element, Linked1Elements) != 0) return null;
            }
            else if (element is PolygonMapElement)
            {
                if (getLinked1Polygons((PolygonMapElement)element, Linked1Elements) != 0) return null;
            }

            return Linked1Elements;
        }

        internal Dictionary<MapElement, int> getLinked2Elements(MapElement element)
        {
            Dictionary<MapElement, int> Linked2Elements = new Dictionary<MapElement, int>();

            if (element is PolygonMapElement)
            {
                if (getLinked2Polygons((PolygonMapElement)element, Linked2Elements) != 0) return null;
            }

            return Linked2Elements;
        }

        private int getLinked1Polygons(PointMapElement pointElement, Dictionary<MapElement, int> linked1Polygons)
        {
            double x, y;
            if (!pointElement.Point.getProjection(Projection, out x, out y))
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, "Projection error");
                return -1;
            }

            getLinkedPolygons(x, y, linked1Polygons);
            return 0;
        }

        private int getLinked1Lines(PointMapElement pointElement, Dictionary<MapElement, int> linked1Lines)
        {
            foreach (LineMapElement lineElement in _elementDictionary.Values.OfType<LineMapElement>())
            {
                if (lineElement.getDistance(pointElement.Point, Resolution) < LinePointLinkThreshold && !linked1Lines.ContainsKey(lineElement)) linked1Lines.Add(lineElement, 0);
            }

            return 0;
        }

        private int getLinked1Lines(LineMapElement lineElement, Dictionary<MapElement, int> linked1Lines)
        {
            if (_attachmentDictionary.ContainsKey(lineElement))
            {
                foreach (LineMapElement linkedLine in _attachmentDictionary[lineElement].Keys)
                {
                    if (!linked1Lines.ContainsKey(linkedLine)) linked1Lines.Add(linkedLine, 0);
                }
            }

            return 0;
        }

        private int getLinked1Polygons(LineMapElement lineElement, Dictionary<MapElement, int> linked1Polygons)
        {
            List<GeoPoint> points = lineElement.getPoints(Resolution);
            foreach (GeoPoint point in points)
            {
                double x, y;
                if (!point.getProjection(Projection, out x, out y))
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, "Projection error");
                    return -1;
                }

                getLinkedPolygons(x, y, linked1Polygons);
            }

            return 0;
        }

        private int getLinked1Polygons(PolygonMapElement polygonElement, Dictionary<MapElement, int> linked1Polygons)
        {
            foreach (string elementId in polygonElement.CoveredElementList)
            {
                MapElement element;
                if (_elementDictionary.TryGetValue(elementId, out element))
                {
                    if (element is PolygonMapElement && !linked1Polygons.ContainsKey((PolygonMapElement)element)) linked1Polygons.Add((PolygonMapElement)element, 0);
                }
            }

            return 0;
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

        private int getLinked2Polygons(PolygonMapElement polygonElement, Dictionary<MapElement, int> linked2Polygons)
        {
            if (_neighborPolygonDictionary.ContainsKey(polygonElement))
            {
                foreach (PolygonMapElement neighborPolygon in _neighborPolygonDictionary[polygonElement].Keys)
                {
                    if (!linked2Polygons.ContainsKey(neighborPolygon)) linked2Polygons.Add(neighborPolygon, 0);
                }
            }

            return 0;
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
