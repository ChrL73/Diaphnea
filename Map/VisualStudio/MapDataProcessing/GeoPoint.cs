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
    }
}
