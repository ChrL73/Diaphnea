using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class GeoPoint
    {
        private readonly double _x;
        private readonly double _y;
        private readonly double _z;

        public GeoPoint(double longitude, double latitude)
        {
            const double a = 3.1415926535897932384626433832795 / 180.0;
            longitude *= a;
            latitude *= a;
            double r = 6371.0;

            _x = r * Math.Cos(longitude) * Math.Cos(latitude);
            _y = r * Math.Sin(longitude) * Math.Cos(latitude);
            _z = r * Math.Sin(latitude);
        }

        internal BsonDocument getBsonDocument()
        {
            BsonDocument pointDocument = new BsonDocument()
            {
                { "x", _x },
                { "y", _y },
                { "z", _z }
            };

            return pointDocument;
        }
    }
}
