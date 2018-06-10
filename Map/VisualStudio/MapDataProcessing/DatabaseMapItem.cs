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
        //private readonly string _element0Id;
        private readonly string _type;

        private static int _generationCounter = 0;
        private static int _instanceCounter = 0;

        internal DatabaseMapItem(bool useBezierSucessors/*, string element0Id*/)
        {
            _useBezierSucessors = useBezierSucessors;
            //_element0Id = element0Id;

            Cap1Round = true;
            Cap2Round = true;

            if (useBezierSucessors) _type = "polygon";
            else _type = "line";

            ++_instanceCounter;
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

        internal int CppOffset { get; private set; }
        internal bool Cap1Round { private get; set; }
        internal bool Cap2Round { private get; set; }

        internal List<GeoPoint> getPointList(XmlResolution resolution)
        {
            List<GeoPoint> list;
            if (_lineDictionary.TryGetValue(resolution, out list)) return list;
            return null;
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

                lineList.Add(pointList);
            }

            if (_generationCounter == 0) Console.Write("0%");
            ++_generationCounter;

            CppOffset = codeGenerator.addMultipointItem(xMin, xMax, yMin, yMax, _itemId.Value, itemName, lineList, Cap1Round, Cap2Round, _type);

            Console.Write(String.Format("\r{0}%", 100 * _generationCounter / _instanceCounter));

            return 0;
        }
    }
}
