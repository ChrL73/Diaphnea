using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class GeoPoint
    {
        private readonly double _longitude;
        private readonly double _latitude;
        private readonly double _x;
        private readonly double _y;
        private readonly double _z;

        internal GeoPoint(double longitude, double latitude)
        {
            _longitude = longitude;
            _latitude = latitude;

            const double a = 3.1415926535897932384626433832795 / 180.0;
            longitude *= a;
            latitude *= a;
            double r = 6371.0;

            _x = r * Math.Cos(longitude) * Math.Cos(latitude);
            _y = r * Math.Sin(longitude) * Math.Cos(latitude);
            _z = r * Math.Sin(latitude);

            if (!_mapCenterSet) updateMinAndMax();
        }

        internal GeoPoint(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;

            double r = Math.Sqrt(x * x + y * y + z * z);
            x /= r;
            y /= r;
            z /= r;

            const double b = 180.0 / 3.1415926535897932384626433832795;
            _latitude = Math.Asin(z);
            _longitude = b * Math.Asin(y / Math.Cos(_latitude));
            _latitude *= b;

            if (!_mapCenterSet) updateMinAndMax();
        }

        internal double Longitude { get { return _longitude; } }
        internal double Latitude { get { return _latitude; } }
        internal double X { get { return _x; } }
        internal double Y { get { return _y; } }
        internal double Z { get { return _z; } }

        static internal double distance(GeoPoint p1, GeoPoint p2)
        {
            double dx = p1._x - p2._x;
            double dy = p1._y - p2._y;
            double dz = p1._z - p2._z;

            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        internal BsonDocument getBsonDocument()
        {
            BsonDocument pointDocument = new BsonDocument()
            {
                { "x", _x},
                { "y", _y},
                { "z", _z},
                { "lo", _longitude},
                { "la", _latitude},
            };

            return pointDocument;
        }

        private class Pair
        {
            internal double X { get; set; }
            internal double Y { get; set; }
        }

        private readonly Dictionary<XmlProjectionEnum, Pair> _projectionDictionary = new Dictionary<XmlProjectionEnum, Pair>();

        static private double? _xMin = null;
        static private double? _xMax = null;
        static private double? _yMin = null;
        static private double? _yMax = null;
        static private double? _zMin = null;
        static private double? _zMax = null;
        static private bool _mapCenterSet = false;
        static private double _uX1, _uX2, _uX3;
        static private double _uY1, _uY2, _uY3;

        static internal void reset()
        {
            _xMin = null;
            _xMax = null;
            _yMin = null;
            _yMax = null;
            _zMin = null;
            _zMax = null;
            _mapCenterSet = false;
        }

        private void updateMinAndMax()
        {
            if (_xMin == null || _x < _xMin) _xMin = _x;
            if (_xMax == null || _x > _xMax) _xMax = _x;
            if (_yMin == null || _y < _yMin) _yMin = _y;
            if (_yMax == null || _y > _yMax) _yMax = _y;
            if (_zMin == null || _z < _zMin) _zMin = _z;
            if (_zMax == null || _z > _zMax) _zMax = _z;
        }

        internal bool getProjection(XmlProjectionEnum projection, out double X, out double Y)
        {
            if (!_mapCenterSet) setMapCenter();

            Pair pair;
            if (_projectionDictionary.TryGetValue(projection, out pair))
            {
                X = pair.X;
                Y = pair.Y;
                return true;
            }


            throw new NotImplementedException();
        }

        static private void setMapCenter()
        {
            _mapCenterSet = true;

            double refX = 0.5 * (_xMin.Value + _xMax.Value);
            double refY = 0.5 * (_yMin.Value + _yMax.Value);
            double refZ = 0.5 * (_zMin.Value + _zMax.Value);
            GeoPoint refPoint = new GeoPoint(refX, refY, refZ);

            const double a = 3.1415926535897932384626433832795 / 180.0;
            double refLon = refPoint.Longitude * a;
            double refLat = refPoint.Latitude * a;

            _uX1 = -Math.Sin(refLon);
            _uX2 = Math.Cos(refLon);
            _uX3 = 0.0;
            _uY1 = -Math.Cos(refLon) * Math.Sin(refLat);
            _uY2 = -Math.Sin(refLon) * Math.Sin(refLat);
            _uY3 = Math.Cos(refLat);
        }
    }
}
