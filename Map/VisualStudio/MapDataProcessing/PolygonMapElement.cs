using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class PolygonMapElement : MapElement
    {
        private readonly List<KmlFileData> _pointKmlFileList = new List<KmlFileData>();
        private readonly List<KmlFileData> _lineKmlFileList = new List<KmlFileData>();
        private readonly List<KmlFileData> _polygonKmlFileList = new List<KmlFileData>();

        private readonly Dictionary<KmlFileData, List<OrientedPolygonLinePart>> _linePartDictionary = new Dictionary<KmlFileData, List<OrientedPolygonLinePart>>();
        private readonly List<OrientedLineList> _compoundPolygonList = new List<OrientedLineList>();
        private readonly List<PolygonPolygonPart> _polygonPartList = new List<PolygonPolygonPart>();
        private readonly DatabaseMapItem _contourMapItem = new DatabaseMapItem();
        private readonly Dictionary<XmlResolution, SortedDictionary<double, List<GeoPoint>>> _subPolygonDictionary = new Dictionary<XmlResolution, SortedDictionary<double, List<GeoPoint>>>();

        internal PolygonMapElement(String id, MapData mapData) : base(id, mapData) { }

        internal override int addKmlFile(String path)
        {
            KmlFileData data = KmlFileData.getData(path);
            if (data.Type == KmlFileTypeEnum.POINT) _pointKmlFileList.Add(data);
            if (data.Type == KmlFileTypeEnum.LINE) _lineKmlFileList.Add(data);
            if (data.Type == KmlFileTypeEnum.POLYGON) _polygonKmlFileList.Add(data);

            return 0;
        }

        internal override int formParts()
        {
            if (_pointKmlFileList.Count == 0 && _lineKmlFileList.Count == 0 && _polygonKmlFileList.Count == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("No Kml file associated to element '{0}'", Id));
                return -1;
            }

            if (_pointKmlFileList.Count != _lineKmlFileList.Count)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Point count ({0}) should be equal to line count ({1}) for element '{2}'",
                                                                              _pointKmlFileList.Count, _lineKmlFileList.Count, Id));
                return -1;
            }

            if (_pointKmlFileList.Count == 1)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Point count ({0}) can not be equal to 1 for element '{1}'",
                                                                              _pointKmlFileList.Count, Id));
                return -1;
            }

            double dMaxInKm = MapData.XmlMapData.parameters.maxConnectionDistanceInKm;

            foreach (KmlFileData line in _lineKmlFileList)
            {
                KmlFileData point1 = getNearestPoint(line.PointList[0]);
                if (point1 == null)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can't associate point1 to line '{0}' for element '{1}' with maxConnectionDistanceInKm={2}",
                                                                                  line.Path, Id, dMaxInKm));
                    return -1;
                }

                KmlFileData point2 = getNearestPoint(line.PointList[line.PointList.Count - 1]);
                if (point2 == null)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can't associate point2 to line '{0}' for element '{1}' with maxConnectionDistanceInKm={2}",
                                                                                  line.Path, Id, dMaxInKm));
                    return -1;
                }

                PolygonLinePart part = PolygonLinePart.getPart(line, point1, point2);
                if (part == null) return -1;

                OrientedPolygonLinePart directPart = new OrientedPolygonLinePart(part, OrientationEnum.DIRECT);
                OrientedPolygonLinePart inversePart = new OrientedPolygonLinePart(part, OrientationEnum.INVERSE);

                if (!_linePartDictionary.ContainsKey(point1)) _linePartDictionary.Add(point1, new List<OrientedPolygonLinePart>());
                _linePartDictionary[point1].Add(directPart);

                if (!_linePartDictionary.ContainsKey(point2)) _linePartDictionary.Add(point2, new List<OrientedPolygonLinePart>());
                _linePartDictionary[point2].Add(inversePart);
            }

            if (_linePartDictionary.Count != _lineKmlFileList.Count)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Fail to form parts for element '{0}': {1} lines in element, {2} parts formed",
                                                                              Id, _lineKmlFileList.Count, _linePartDictionary.Count));
                return -1;
            }

            foreach (KeyValuePair<KmlFileData, List<OrientedPolygonLinePart>> pair in _linePartDictionary)
            {
                if (pair.Value.Count != 2)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Fail to form parts for element '{0}': {1} oriented lines instead of 2 for point '{2}'",
                                                                                  Id, pair.Value.Count, pair.Key.Path));
                    return -1;
                }
            }

            while (_linePartDictionary.Count != 0)
            {
                OrientedPolygonLinePart startPart = _linePartDictionary.FirstOrDefault().Value[0];
                OrientedPolygonLinePart nextPart = startPart;

                OrientedLineList orientedLineList = new OrientedLineList();
                List<KmlFileData> keysToRemove = new List<KmlFileData>();

                while (true)
                {
                    orientedLineList.LineList.Add(nextPart);
                    keysToRemove.Add(nextPart.Point1);
                    nextPart = getNextPart(nextPart);
                    if (nextPart == startPart || nextPart == null) break;
                }

                if (nextPart == null)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Fail to sort parts for element '{0}' (Next part not found)", Id));
                    return -1;
                }

                foreach (KmlFileData key in keysToRemove) _linePartDictionary.Remove(key);

                _compoundPolygonList.Add(orientedLineList);
            }

            foreach (KmlFileData polygon in _polygonKmlFileList)
            {
                PolygonPolygonPart part = PolygonPolygonPart.getPart(polygon);
                _polygonPartList.Add(part);
            }

            return 0;
        }

        private OrientedPolygonLinePart getNextPart(OrientedPolygonLinePart part)
        {
            List<OrientedPolygonLinePart> partList;
            if (!_linePartDictionary.TryGetValue(part.Point2, out partList)) return null;

            if (partList[0].Line == part.Line && partList[1].Line != part.Line) return partList[1];
            if (partList[0].Line != part.Line && partList[1].Line == part.Line) return partList[0];

            return null;
        }

        private KmlFileData getNearestPoint(GeoPoint point)
        {
            KmlFileData result = null;
            double dMin = MapData.XmlMapData.parameters.maxConnectionDistanceInKm;

            foreach (KmlFileData kmlFileData in _pointKmlFileList)
            {
                GeoPoint p = kmlFileData.PointList[0];
                double d = GeoPoint.distance(p, point);
                if (d < dMin)
                {
                    result = kmlFileData;
                    dMin = d;
                }
            }

            return result;
        }

        internal int formContours()
        {
            foreach (XmlResolution resolution in MapData.XmlMapData.resolutionList)
            {
                List<List<GeoPoint>> polygonList = new List<List<GeoPoint>>();
                SortedDictionary<double, List<GeoPoint>> polygonSortedDictionary = new SortedDictionary<double, List<GeoPoint>>();

                foreach (OrientedLineList lineList in _compoundPolygonList)
                {
                    List<GeoPoint> list = new List<GeoPoint>();
                    foreach (OrientedPolygonLinePart part in lineList.LineList)
                    {
                        List<GeoPoint> subList = part.getPointList(resolution);
                        list.AddRange(subList);
                        list.RemoveAt(list.Count - 1);
                    }

                    polygonList.Add(list);

                    List<GeoPoint> list2 = new List<GeoPoint>(list);
                    list2.Add(list2[0]);
                    polygonSortedDictionary.Add(-Smoother.getLineLength(list2), list2);
                }

                foreach (PolygonPolygonPart part in _polygonPartList)
                {
                    List<GeoPoint> list = new List<GeoPoint>(part.getPointList(resolution));
                    polygonSortedDictionary.Add(-Smoother.getLineLength(list), new List<GeoPoint>(list));
                    list.RemoveAt(list.Count - 1);
                    polygonList.Add(list);
                }

                List<GeoPoint> contour = polygonList[0];
                GeoPoint p = contour[contour.Count - 1];
                int i, n = polygonList.Count;
                for (i = 1; i < n; ++i)
                {
                    contour.AddRange(polygonList[i]);
                    contour.Add(polygonList[i][0]);
                    contour.Add(p);
                }
                contour.Add(contour[0]);

                _contourMapItem.addLine(resolution, contour);
                _subPolygonDictionary.Add(resolution, polygonSortedDictionary);

                int index = 1;
                foreach (List<GeoPoint> pointList in polygonSortedDictionary.Values)
                {
                    if (polygonSortedDictionary.Count > 1) KmlWriter.write(pointList, KmlFileTypeEnum.POLYGON, "Polygons", String.Format("{0}_#{1}.kml", Id, index), resolution);
                    else KmlWriter.write(pointList, KmlFileTypeEnum.POLYGON, "Polygons", String.Format("{0}.kml", Id), resolution);
                    ++index;
                }
            }

            return 0;
        }

        internal override int fillDatabase(IMongoDatabase database)
        {
            _contourMapItem.fillDataBase(database, MapData, Id);

            IMongoCollection<BsonDocument> polygonElementCollection = database.GetCollection<BsonDocument>("polygon_elements");

            BsonArray itemArray = new BsonArray();

            foreach (OrientedLineList list in _compoundPolygonList)
            {
                foreach (OrientedPolygonLinePart linePart in list.LineList)
                {
                    itemArray.Add(linePart.MapItemId);
                }
            }

            BsonDocument elementDocument = new BsonDocument()
            {
                { "map", MapData.XmlMapData.parameters.mapId },
                { "id", Id },
                { "contour", _contourMapItem.Id },
                { "items", itemArray}
            };

            polygonElementCollection.InsertOne(elementDocument);

            return 0;
        }
    }
}
