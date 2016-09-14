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

        private readonly Dictionary<KmlFileData, OrientedPolygonPart> _partDictionary = new Dictionary<KmlFileData, OrientedPolygonPart>();
        private readonly List<OrientedPolygonPart> _sortedPartList = new List<OrientedPolygonPart>();

        private readonly Dictionary<KmlFileData, OrientedPolygonPart> _lineToPartDictionary = new Dictionary<KmlFileData, OrientedPolygonPart>();
        private readonly Dictionary<KmlFileData, OrientedPolygonPart> _point1ToPartDictionary = new Dictionary<KmlFileData, OrientedPolygonPart>();
        private readonly Dictionary<KmlFileData, OrientedPolygonPart> _point2ToPartDictionary = new Dictionary<KmlFileData, OrientedPolygonPart>();
        
        internal PolygonMapElement(String id) : base(id) { }

        internal override int addKmlFile(String path)
        {
            KmlFileData data = KmlFileData.getData(path);
            if (data.Type == KmlFileTypeEnum.POINT) _pointKmlFileList.Add(data);
            if (data.Type == KmlFileTypeEnum.LINE) _lineKmlFileList.Add(data);
            if (data.Type == KmlFileTypeEnum.POLYGON) _polygonKmlFileList.Add(data);

            return 0;
        }

        internal override int formParts(double dMaxInKm)
        {
            if (_pointKmlFileList.Count != _lineKmlFileList.Count)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Point count ({0}) should be equal to line count ({1}) for element '{2}'",
                                                                              _pointKmlFileList.Count, _lineKmlFileList.Count, Id));
                return -1;
            }

            if (_pointKmlFileList.Count <= 1)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Point count ({0}) should be greater than 1 for element '{1}'",
                                                                              _pointKmlFileList.Count, Id));
                return -1;
            }

            OrientedPolygonPart startPart = null;

            foreach (KmlFileData line in _lineKmlFileList)
            {
                KmlFileData point1 = getNearestPoint(line.PointList[0], dMaxInKm);
                if (point1 == null)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can't associate point1 to line '{0}' for element '{1}' with maxConnectionDistanceInKm={2}",
                                                                                  line.Path, Id, dMaxInKm));
                    return -1;
                }

                KmlFileData point2 = getNearestPoint(line.PointList[line.PointList.Count - 1], dMaxInKm);
                if (point2 == null)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can't associate point2 to line '{0}' for element '{1}' with maxConnectionDistanceInKm={2}",
                                                                                  line.Path, Id, dMaxInKm));
                    return -1;
                }

                PolygonPart part = PolygonPart.getPart(line, point1, point2);
                if (part == null) return -1;

                OrientedPolygonPart directPart = new OrientedPolygonPart(part, OrientationEnum.DIRECT);
                OrientedPolygonPart inversePart = new OrientedPolygonPart(part, OrientationEnum.INVERSE);
                if (startPart == null) startPart = directPart;

                _partDictionary.Add(point1, directPart);
                _partDictionary.Add(point2, inversePart);
            }

            return 0;
        }

        private KmlFileData getNearestPoint(GeoPoint point, double dMaxInKm)
        {
            KmlFileData result = null;
            double dMin = dMaxInKm;

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
    }
}
