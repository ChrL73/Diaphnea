using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class SuperposerSegment
    {
        private readonly List<GeoPoint> _line;
        private readonly int _i1;
        private readonly GeoPoint _A1;
        private readonly GeoPoint _A2;
        private readonly double _ux;
        private readonly double _uy;
        private readonly double _uz;
        private readonly double _r;

        internal GeoPoint A1 { get { return _A1; } }
        internal GeoPoint A2 { get { return _A2; } }

        internal SuperposerSegment(List<GeoPoint> line, int i1)
        {
            _line = line;
            _i1 = i1;
            _A1 = line[i1];
            _A2 = line[i1 + 1];

            _ux = _A2.X - _A1.X;
            _uy = _A2.Y - _A1.Y;
            _uz = _A2.Z - _A1.Z;
            _r = Math.Sqrt(_ux * _ux + _uy * _uy + _uz * _uz);

            _ux /= _r;
            _uy /= _r;
            _uz /= _r;
        }

        internal double getDistance(GeoPoint B)
        {
            double vx = B.X - _A1.X;
            double vy = B.Y - _A1.Y;
            double vz = B.Z - _A1.Z;
            double a = vx * _ux + vy * _uy + vz * _uz;

            if (a <= 0) return DistanceCalculator.getDistance(_A1, B);
            if (a >= _r) return DistanceCalculator.getDistance(_A2, B);

            GeoPoint C = new GeoPoint(_A1.X + a * _ux, _A1.Y + a * _uy, _A1.Z + a * _uz);
            return DistanceCalculator.getDistance(C, B);
        }

        internal SuperposerPoint getNearestExtremity(GeoPoint B)
        {
            if (DistanceCalculator.getDistance(_A1, B) > DistanceCalculator.getDistance(_A2, B)) return new SuperposerPoint(_line, _i1 + 1);
            return new SuperposerPoint(_line, _i1);
        }
    }
}
