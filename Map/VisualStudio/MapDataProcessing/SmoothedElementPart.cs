using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class SmoothedElementPart
    {
        private readonly Dictionary<XmlResolution, List<GeoPoint>> _lineDictionary = new Dictionary<XmlResolution, List<GeoPoint>>();

        internal SmoothedElementPart(OrientedPolygonPart part, MapData mapData)
        {
            List<GeoPoint> originalLine = new List<GeoPoint>();

            originalLine.Add(part.Point1.PointList[0]);

            if (part.Orientation == OrientationEnum.DIRECT)
            {
                originalLine.AddRange(part.Line.PointList);
            }
            else
            {
                List<GeoPoint> reversedList = new List<GeoPoint>(part.Line.PointList);
                reversedList.Reverse();
                originalLine.AddRange(reversedList);
            }

            originalLine.Add(part.Point2.PointList[0]);

            smooth(originalLine);
        }

        // Todo: Another constructor for line elements, which calls the same smooth() method (?)
        // Todo: Another constructor for polygon parts of polygons, which calls another smooth() method.

        private void smooth(List<GeoPoint> originalLine)
        {

        }
    }
}
