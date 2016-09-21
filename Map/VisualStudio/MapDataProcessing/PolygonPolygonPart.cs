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
        private readonly Dictionary<XmlResolution, List<GeoPoint>> _smoothedPolygonDictionary = new Dictionary<XmlResolution, List<GeoPoint>>();

        private PolygonPolygonPart(KmlFileData polygonData)
        {
            _polygonData = polygonData;
        }

        internal KmlFileData Polygon { get { return _polygonData; } }

        internal static int smoothAll(MapData mapData)
        {
            foreach (PolygonPolygonPart part in _partDictionary.Values)
            {
                List<GeoPoint> line = new List<GeoPoint>(part.Polygon.PointList);

                foreach (XmlResolution resolution in mapData.XmlMapData.resolutionList)
                {
                    List<GeoPoint> smoothedPolygon = Smoother.smoothPolygon(line, resolution, part.Polygon.Path);
                    if (smoothedPolygon == null) return -1;
                    part._smoothedPolygonDictionary.Add(resolution, smoothedPolygon);
                    KmlWriter.write(smoothedPolygon, KmlFileTypeEnum.POLYGON, "Polygons_Polygons", Path.GetFileName(part.Polygon.Path), resolution);
                }
            }

            return 0;
        }

    }
}
