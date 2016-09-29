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
    class DatabaseMapItem
    {
        private readonly Dictionary<XmlResolution, List<GeoPoint>> _lineDictionary = new Dictionary<XmlResolution,List<GeoPoint>>();

        internal void addLine(XmlResolution resolution, List<GeoPoint> line)
        {
            _lineDictionary.Add(resolution, line);
        }

        internal BsonValue Id { get; set; }

        internal List<GeoPoint> getPointList(XmlResolution resolution)
        {
            List<GeoPoint> list;
            if (_lineDictionary.TryGetValue(resolution, out list)) return list;
            return null;
        }

        internal int fillDataBase(IMongoDatabase database, MapData mapData, string itemName)
        {
            IMongoCollection<BsonDocument> pointListCollection = database.GetCollection<BsonDocument>("point_lists");

            foreach (KeyValuePair<XmlResolution, List<GeoPoint>> pair in _lineDictionary)
            {
                List<GeoPoint> list = pair.Value;
                double resolution = pair.Key.sampleLength1 * Double.Parse(pair.Key.sampleRatio);

                BsonArray pointArray = new BsonArray();
                foreach (GeoPoint point in list)
                {
                    pointArray.Add(point.getBsonDocument());
                }

                BsonDocument pointListDocument = new BsonDocument()
                    {
                        { "map", mapData.XmlMapData.parameters.mapId },
                        { "item", itemName },
                        { "resolution", resolution },
                        { "count", list.Count },
                        { "points", pointArray }
                    };

                pointListCollection.InsertOne(pointListDocument);
                //list.Id = pointListDocument.GetValue("_id");
            }

            return 0;
        }
    }
}
