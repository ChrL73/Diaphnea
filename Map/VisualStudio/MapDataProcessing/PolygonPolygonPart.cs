using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class PolygonPolygonPart
    {
        private static Dictionary<KmlFileData, PolygonPolygonPart> _partDictionary = new Dictionary<KmlFileData, PolygonPolygonPart>();

        internal static PolygonPolygonPart getPart(KmlFileData polygonData)
        {
            PolygonPolygonPart part;
            if (!_partDictionary.TryGetValue(polygonData, out part))
            {
                part = new PolygonPolygonPart(polygonData);
                _partDictionary.Add(polygonData, part);
            }

            return part;
        }

        private readonly KmlFileData _polygonData;
        private readonly Dictionary<XmlResolution, DatabasePointList> _smoothedPolygonDictionary = new Dictionary<XmlResolution, DatabasePointList>();

        private PolygonPolygonPart(KmlFileData polygonData)
        {
            _polygonData = polygonData;
        }

        internal KmlFileData Polygon { get { return _polygonData; } }

        internal List<GeoPoint> getPointList(XmlResolution resolution)
        {
            DatabasePointList list;
            if (_smoothedPolygonDictionary.TryGetValue(resolution, out list)) return list.PointList;
            return null;
        }

        internal static int smoothAll(MapData mapData)
        {
            foreach (PolygonPolygonPart part in _partDictionary.Values)
            {
                List<GeoPoint> line = new List<GeoPoint>(part.Polygon.PointList);

                foreach (XmlResolution resolution in mapData.XmlMapData.resolutionList)
                {
                    List<GeoPoint> smoothedPolygon = Smoother.smoothPolygon(line, resolution, part.Polygon.Path);
                    if (smoothedPolygon == null) return -1;
                    part._smoothedPolygonDictionary.Add(resolution, new DatabasePointList(smoothedPolygon));
                    KmlWriter.write(smoothedPolygon, KmlFileTypeEnum.POLYGON, "Polygons_Polygons", Path.GetFileName(part.Polygon.Path), resolution);
                }
            }

            return 0;
        }

        internal static int fillDatabase(IMongoDatabase database)
        {
            IMongoCollection<BsonDocument> pointListCollection = database.GetCollection<BsonDocument>("point_lists");

            foreach (PolygonPolygonPart part in _partDictionary.Values)
            {
                foreach (KeyValuePair<XmlResolution, DatabasePointList> pair in part._smoothedPolygonDictionary)
                {
                    DatabasePointList list = pair.Value;
                    double resolution = pair.Key.sampleLength1 * Double.Parse(pair.Key.sampleRatio);

                    BsonArray pointArray = new BsonArray();
                    foreach (GeoPoint point in list.PointList)
                    {
                        pointArray.Add(point.getBsonDocument());
                    }

                    BsonDocument pointListDocument = new BsonDocument()
                    {
                        { "item", Path.GetFileNameWithoutExtension(part.Polygon.Path) },
                        { "resolution", resolution },
                        { "count", list.PointList.Count },
                        { "points", pointArray }
                    };

                    pointListCollection.InsertOne(pointListDocument);
                    list.Id = pointListDocument.GetValue("_id");
                }
            }

            return 0;
        }

    }
}
