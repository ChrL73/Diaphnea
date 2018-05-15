using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

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

        internal GeoPoint(GeoPoint point)
        {
            _x = point._x;
            _y = point._y;
            _z = point._z;
            _latitude = point._latitude;
            _longitude = point._longitude;
        }

        internal double Longitude { get { return _longitude; } }
        internal double Latitude { get { return _latitude; } }
        internal double X { get { return _x; } }
        internal double Y { get { return _y; } }
        internal double Z { get { return _z; } }

        internal GeoPoint BezierSuccessor { get; set; }
        internal GeoPoint BezierPredecessor { get; set; }

        static internal double distance(GeoPoint p1, GeoPoint p2)
        {
            double dx = p1._x - p2._x;
            double dy = p1._y - p2._y;
            double dz = p1._z - p2._z;

            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        internal BsonDocument getBsonDocument(GeoPoint previous2Point, GeoPoint previous1Point, GeoPoint nextPoint, XmlProjectionEnum projection)
        {
            double x, y;
            if (!getProjection(projection, out x, out y)) return null;

            BsonDocument pointDocument = null;

            if (previous1Point == null)
            {
                pointDocument = new BsonDocument()
                {
                    { "x", x },
                    { "y", y }
                };
            }
            else
            {
                double xP1, yP1;
                if (!previous1Point.getProjection(projection, out xP1, out yP1)) return null;

                double x1 = xP1;
                double y1 = yP1;
                double x2 = x;
                double y2 = y;

                double dx = x - xP1;
                double dy = y - yP1;
                double d = Math.Sqrt(dx * dx + dy * dy);

                if (previous2Point != null)
                {
                    double xP2, yP2;
                    if (!previous2Point.getProjection(projection, out xP2, out yP2)) return null;

                    double ux = x - xP2;
                    double uy = y - yP2;
                    double r = Math.Sqrt(ux * ux + uy * uy);
                    ux /= r;
                    uy /= r;

                    dx = xP1 - xP2;
                    dy = yP1 - yP2;
                    double d2 = Math.Sqrt(dx * dx + dy * dy);
                    if (d < d2) d2 = d;

                    double cos = getCos(xP1, yP1, xP2, yP2, x, y);
                    double q = d2 * 0.1 * (3.0 - cos);

                    x1 = xP1 + q * ux;
                    y1 = yP1 + q * uy;
                }

                if (nextPoint != null)
                {
                    double xN, yN;
                    if (!nextPoint.getProjection(projection, out xN, out yN)) return null;

                    double ux = xP1 - xN;
                    double uy = yP1 - yN;
                    double r = Math.Sqrt(ux * ux + uy * uy);
                    ux /= r;
                    uy /= r;

                    dx = x - xN;
                    dy = y - yN;
                    double d2 = Math.Sqrt(dx * dx + dy * dy);
                    if (d < d2) d2 = d;

                    double cos = getCos(x, y, xP1, yP1, xN, yN);
                    double q = d2 * 0.1 * (3.0 - cos);

                    x2 = x + q * ux;
                    y2 = y + q * uy;
                }

                pointDocument = new BsonDocument()
                {
                    { "x1", x1 },
                    { "y1", y1 },
                    { "x2", x2 },
                    { "y2", y2 },
                    { "x", x },
                    { "y", y }
                };
            }

            return pointDocument;
        }

        internal List<double> getInfo(GeoPoint previous2Point, GeoPoint previous1Point, GeoPoint nextPoint, XmlProjectionEnum projection)
        {
            double x, y;
            if (!getProjection(projection, out x, out y)) return null;

            List<double> info = new List<double>();

            if (previous1Point == null)
            {
                info.Add(0.0);
                info.Add(0.0);
                info.Add(0.0);
                info.Add(0.0);
                info.Add(x);
                info.Add(-y);
            }
            else
            {
                double xP1, yP1;
                if (!previous1Point.getProjection(projection, out xP1, out yP1)) return null;

                double x1 = xP1;
                double y1 = yP1;
                double x2 = x;
                double y2 = y;

                double dx = x - xP1;
                double dy = y - yP1;
                double d = Math.Sqrt(dx * dx + dy * dy);

                if (previous2Point != null)
                {
                    double xP2, yP2;
                    if (!previous2Point.getProjection(projection, out xP2, out yP2)) return null;

                    double ux = x - xP2;
                    double uy = y - yP2;
                    double r = Math.Sqrt(ux * ux + uy * uy);
                    ux /= r;
                    uy /= r;

                    dx = xP1 - xP2;
                    dy = yP1 - yP2;
                    double d2 = Math.Sqrt(dx * dx + dy * dy);
                    if (d < d2) d2 = d;

                    double cos = getCos(xP1, yP1, xP2, yP2, x, y);
                    double q = d2 * 0.1 * (3.0 - cos);

                    x1 = xP1 + q * ux;
                    y1 = yP1 + q * uy;
                }

                if (nextPoint != null)
                {
                    double xN, yN;
                    if (!nextPoint.getProjection(projection, out xN, out yN)) return null;

                    double ux = xP1 - xN;
                    double uy = yP1 - yN;
                    double r = Math.Sqrt(ux * ux + uy * uy);
                    ux /= r;
                    uy /= r;

                    dx = x - xN;
                    dy = y - yN;
                    double d2 = Math.Sqrt(dx * dx + dy * dy);
                    if (d < d2) d2 = d;

                    double cos = getCos(x, y, xP1, yP1, xN, yN);
                    double q = d2 * 0.1 * (3.0 - cos);

                    x2 = x + q * ux;
                    y2 = y + q * uy;
                }

                info.Add(x1);
                info.Add(-y1);
                info.Add(x2);
                info.Add(-y2);
                info.Add(x);
                info.Add(-y);
            }

            return info;
        }

        private double getCos(double xO, double yO, double xA, double yA, double xB, double yB)
        {
            double ux = xA - xO;
            double uy = yA - yO;
            double r = Math.Sqrt(ux * ux + uy * uy);
            ux /= r;
            uy /= r;

            double vx = xB - xO;
            double vy = yB - yO;
            r = Math.Sqrt(vx * vx + vy * vy);
            vx /= r;
            vy /= r;

            return ux * vx + uy * vy;
        }

        private class Pair
        {
            private readonly double _x;
            private readonly double _y;

            internal Pair(double x, double y) { _x = x; _y = y; }

            internal double X { get { return _x; } }
            internal double Y { get { return _y; } }
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

        static internal double? XMin { get { return _xMin; } }
        static internal double? XMax { get { return _xMax; } }
        static internal double? YMin { get { return _yMin; } }
        static internal double? YMax { get { return _yMax; } }
        static internal double? ZMin { get { return _zMin; } }
        static internal double? ZMax { get { return _zMax; } }

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

            if (projection == XmlProjectionEnum.ORTHOGRAPHIC)
            {
                X = _x * _uX1 + _y * _uX2 + _z * _uX3;
                Y = _x * _uY1 + _y * _uY2 + _z * _uY3;
                _projectionDictionary.Add(projection, new Pair(X, Y));
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
