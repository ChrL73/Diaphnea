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
        private readonly bool _useBezierSucessors;
        private readonly string _element0Id;

        internal DatabaseMapItem(bool useBezierSucessors, string element0Id)
        {
            _useBezierSucessors = useBezierSucessors;
            _element0Id = element0Id;
            Cap1Round = true;
            Cap2Round = true;
        }

        internal void addLine(XmlResolution resolution, List<GeoPoint> line)
        {
            _lineDictionary.Add(resolution, line);
        }

        internal BsonValue Id { get; set; }
        internal bool Cap1Round { private get; set; }
        internal bool Cap2Round { private get; set; }

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
                bool closedLine = (DistanceCalculator.getDistance(list[0], list[n - 1]) < 0.0001);
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
                            previous1Point = list[(n + i - 2) % (n - 1)];

                            if (_useBezierSucessors)
                            {
                                // (This concerns polygon elements and not line elements) 
                                // 'previous1Point' is the previous point in the contour. 'BezierPredecessor' is the previous point in the current polygon of the contour.
                                // If 'previous1Point' is not the 'BezierPredecessor', this means theat the current segment links 2 distincts polygons of the contour.
                                // In this case, we set 'previous2Point' and 'nextPoint' to 'null', so that the segment is a straight line (and not a Bezier curve) in the SVG path.
                                // This straight line will not be visible when the contour will be filled, because it will be identical to another straight line of the contour.
                                if (previous1Point == list[i].BezierPredecessor)
                                {
                                    previous2Point = previous1Point.BezierPredecessor;
                                    nextPoint = list[i].BezierSuccessor;
                                }
                            }
                            else
                            {
                                previous2Point = list[(n + i - 3) % (n - 1)];
                                nextPoint = list[(n + i) % (n - 1)];
                            }
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
                { "element0", _element0Id },
                { "item", itemName },
                { "cap1_round", Cap1Round ? 1 : 0 },
                { "cap2_round", Cap2Round ? 1 : 0 },
                { "point_lists", lineArray }
            };

            itemCollection.InsertOne(itemDocument);

            Id = itemDocument.GetValue("_id");

            return 0;
        }
    }
}
