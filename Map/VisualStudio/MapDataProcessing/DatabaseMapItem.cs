using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

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

        internal List<GeoPoint> getLine(XmlResolution resolution)
        {
            List<GeoPoint> line;
            if (!_lineDictionary.TryGetValue(resolution, out line)) return null;
            return line;
        }

        internal BsonValue Id { get; private set; }
        internal int CppOffset { get; private set; }
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

            double xMin = Double.MaxValue;
            double xMax = Double.MinValue;
            double yMin = Double.MaxValue;
            double yMax = Double.MinValue;

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

                    BsonDocument bsonPoint = list[i].getBsonDocument(previous2Point, previous1Point, nextPoint, mapData.XmlMapData.parameters.projection);
                    pointArray.Add(bsonPoint);

                    double x = bsonPoint.AsBsonDocument.GetElement("x").Value.AsDouble;
                    double y = bsonPoint.AsBsonDocument.GetElement("y").Value.AsDouble;
                    if (x < xMin) xMin = x;
                    if (x > xMax) xMax = x;
                    if (y < yMin) yMin = y;
                    if (y > yMax) yMax = y;
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
                { "x_min", xMin },
                { "x_max", xMax },
                { "y_min", yMin },
                { "y_max", yMax },
                { "cap1_round", Cap1Round ? 1 : 0 },
                { "cap2_round", Cap2Round ? 1 : 0 },
                { "point_lists", lineArray }
            };

            itemCollection.InsertOne(itemDocument);

            Id = itemDocument.GetValue("_id");

            return 0;
        }

        internal int generateCode(CodeGenerator codeGenerator, MapData mapData, string itemName)
        {
            List<List<double>> lineList = new List<List<double>>();

            double xMin = Double.MaxValue;
            double xMax = Double.MinValue;
            double yMin = Double.MaxValue;
            double yMax = Double.MinValue;

            foreach (KeyValuePair<XmlResolution, List<GeoPoint>> pair in _lineDictionary)
            {
                List<GeoPoint> list = pair.Value;

                int i, n = list.Count;
                bool closedLine = (DistanceCalculator.getDistance(list[0], list[n - 1]) < 0.0001);
                List<double> pointList = new List<double>();
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

                    List<double> info = list[i].getInfo(previous2Point, previous1Point, nextPoint, mapData.XmlMapData.parameters.projection);
                    pointList.AddRange(info);

                    double x = info[4];
                    double y = -info[5];

                    if (x < xMin) xMin = x;
                    if (x > xMax) xMax = x;
                    if (y < yMin) yMin = y;
                    if (y > yMax) yMax = y;
                }

                /*BsonDocument pointListDocument = new BsonDocument()
                {
                    { "map", mapData.XmlMapData.parameters.mapId },
                    { "item", itemName },
                    { "resolution", resolution },
                    { "count", list.Count },
                    { "points", pointArray }
                };*/

                //pointListCollection.InsertOne(pointListDocument);

                //lineArray.Add(pointListDocument.GetValue("_id"));
                lineList.Add(pointList);
            }

            /*IMongoCollection<BsonDocument> itemCollection = database.GetCollection<BsonDocument>("items");

            BsonDocument itemDocument = new BsonDocument()
            {
                { "item_id", _itemId.Value },
                { "map", mapData.XmlMapData.parameters.mapId },
                { "element0", _element0Id },
                { "item", itemName },
                { "x_min", xMin },
                { "x_max", xMax },
                { "y_min", yMin },
                { "y_max", yMax },
                { "cap1_round", Cap1Round ? 1 : 0 },
                { "cap2_round", Cap2Round ? 1 : 0 },
                { "point_lists", lineArray }
            };

            itemCollection.InsertOne(itemDocument);

            Id = itemDocument.GetValue("_id");*/

            CppOffset = codeGenerator.addMultipointItem(xMin, xMax, yMin, yMax, _itemId.Value, itemName, lineList, Cap1Round, Cap2Round);

            return 0;
        }
    }
}
