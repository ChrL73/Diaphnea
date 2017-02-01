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
        private readonly ItemId _itemId = new ItemId();
        private readonly Dictionary<XmlResolution, List<GeoPoint>> _lineDictionary = new Dictionary<XmlResolution, List<GeoPoint>>();

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

            BsonArray lineArray = new BsonArray();

            foreach (KeyValuePair<XmlResolution, List<GeoPoint>> pair in _lineDictionary)
            {
                List<GeoPoint> list = pair.Value;
                double resolution = pair.Key.sampleLength1 * Double.Parse(pair.Key.sampleRatio);
                
                int i, n = list.Count;
                bool closedLine = (list[0] == list[n - 1]);
                BsonArray pointArray = new BsonArray();
                for (i = 0; i < n; ++i)
                {
                    GeoPoint previous2Point = null;
                    GeoPoint previous1Point = null;
                    GeoPoint nextPoint = null;

                    if (i != 0)
                    {
                        if (closedLine)
                        {
                            previous2Point = list[(n + i - 3) % (n - 1)];
                            previous1Point = list[(n + i - 2) % (n - 1)];
                            nextPoint = list[(n + i) % (n - 1)];
                        }
                        else
                        {
                            if (i != 1) previous2Point = list[i - 2];
                            previous1Point = list[i - 1];
                            if (i != n - 1) nextPoint = list[i + 1];
                        }
                    }

                    pointArray.Add(list[i].getBsonDocument(previous2Point, previous1Point, nextPoint, mapData.XmlMapData.parameters.projection));
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

                lineArray.Add(pointListDocument.GetValue("_id"));
            }

            IMongoCollection<BsonDocument> itemCollection = database.GetCollection<BsonDocument>("items");

            BsonDocument itemDocument = new BsonDocument()
            {
                { "item_id", _itemId.Value },
                { "map", mapData.XmlMapData.parameters.mapId },
                { "item", itemName },
                { "point_lists", lineArray }
            };

            itemCollection.InsertOne(itemDocument);

            Id = itemDocument.GetValue("_id");

            return 0;
        }
    }
}
