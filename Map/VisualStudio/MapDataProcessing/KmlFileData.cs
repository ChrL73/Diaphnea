using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MapDataProcessing
{
    class KmlFileData
    {
        private static Dictionary<String, KmlFileData> _fileDictionary = new Dictionary<String, KmlFileData>();

        internal static KmlFileData getData(String path)
        {
            KmlFileData data;
            if (!_fileDictionary.TryGetValue(path, out data))
            {
                data = new KmlFileData(path);
                _fileDictionary.Add(path, data);
            }

            return data;
        }

        static CultureInfo _cultureInfo = new CultureInfo("en-US");

        private readonly String _path;
        private readonly KmlFileTypeEnum _type = KmlFileTypeEnum.NONE;
        private readonly List<GeoPoint> _pointList = new List<GeoPoint>();

        private KmlFileData(String path)
        {
            _path = path;

            XmlDocument inputDocument = new XmlDocument();
            try
            {
                inputDocument.Load(path);
            }
            catch (Exception)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Invalid file, {0} ignored", path));
                return;
            }

            XmlNodeList inputCoordinatesList = inputDocument.GetElementsByTagName("coordinates");
            if (inputCoordinatesList.Count == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("No 'coordinates' tags found, file, {0} ignored", path));
                return;
            }

            if (inputCoordinatesList.Count > 1)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("More than one 'coordinates' tags in file {0}. Only the first one is taken into account", path));
            }

            XmlNode inputCoordinatesNode = inputCoordinatesList.Item(0);
            XmlNode parent = inputCoordinatesNode.ParentNode;
            while (parent != null)
            {
                if (parent.Name == "Point")
                {
                    _type = KmlFileTypeEnum.POINT;
                    break;
                }
                else if (parent.Name == "LineString")
                {
                    _type = KmlFileTypeEnum.LINE;
                    break;
                }
                else if (parent.Name == "Polygon")
                {
                    _type = KmlFileTypeEnum.POLYGON;
                    break;
                }
                parent = parent.ParentNode;
            }

            if (_type == KmlFileTypeEnum.NONE)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("'coordinates' tag is not included in either 'Point' or 'LineString' or 'Polygon' tag. {0} ignored", path));
                return;
            }

            String coordinates = inputCoordinatesNode.FirstChild.Value;
            String[] pointArray = coordinates.Split(' ', '\n', '\t');
            int pointCount = 0;

            foreach (String pointStr in pointArray)
            {
                if (pointStr.Length != 0)
                {
                    if (_type == KmlFileTypeEnum.POINT && pointCount != 0)
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Point file must contain only one point, {0} ignored", path));
                        _type = KmlFileTypeEnum.NONE;
                        _pointList.Clear();
                        return;
                    }

                    String[] coordinateArray = pointStr.Split(',');
                    double lon;
                    if (!Double.TryParse(coordinateArray[0], NumberStyles.Number | NumberStyles.AllowExponent, _cultureInfo, out lon))
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Invalid file, can't convert {0} to double, {1} ignored", coordinateArray[0], path));
                        _type = KmlFileTypeEnum.NONE;
                        _pointList.Clear();
                        return;
                    }

                    double lat;
                    if (!Double.TryParse(coordinateArray[1], NumberStyles.Number | NumberStyles.AllowExponent, _cultureInfo, out lat))
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Invalid file, can't convert {0} to double, {1} ignored", coordinateArray[1], path));
                        _type = KmlFileTypeEnum.NONE;
                        _pointList.Clear();
                        return;
                    }

                    _pointList.Add(new GeoPoint(lon, lat));
                    ++pointCount;
                }
            }

            if (_type == KmlFileTypeEnum.LINE && pointCount < 2)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Line file must contain at least 2 points, {0} ignored", path));
                _type = KmlFileTypeEnum.NONE;
                _pointList.Clear();
                return;
            }

            if (_type == KmlFileTypeEnum.POLYGON && pointCount < 4)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Polygon file must contain at least 4 points, {0} ignored", path));
                _type = KmlFileTypeEnum.NONE;
                _pointList.Clear();
                return;
            }
        }

        internal String Path { get { return _path; } }
        internal KmlFileTypeEnum Type { get { return _type; } }
        internal List<GeoPoint> PointList { get { return _pointList; } }
    }
}
