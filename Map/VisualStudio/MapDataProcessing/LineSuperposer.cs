using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class LineSuperposer
    {
        private double _threshold;
        private int _nX;
        private int _nY;
        private int _nZ;
        private double _Dx;
        private double _Dy;
        private double _Dz;
        private double _xMin;
        private double _xMax;
        private double _yMin;
        private double _yMax;
        private double _zMin;
        private double _zMax;

        private readonly List<SuperposerSegment> _segmentList = new List<SuperposerSegment>();
        private readonly Dictionary<int, SuperposerArea> _areaDictionary = new Dictionary<int, SuperposerArea>();

        internal void addReferenceLine(List<GeoPoint> pointList)
        {
            int i, n = pointList.Count;
            for (i = 0; i < n - 1; ++i) _segmentList.Add(new SuperposerSegment(pointList[i], pointList[i + 1]));
        }

        internal void classifySegments(double threshold)
        {
            _threshold = threshold;

            _xMin = GeoPoint.XMin.Value;
            _xMax = GeoPoint.XMax.Value;
            _yMin = GeoPoint.YMin.Value;
            _yMax = GeoPoint.YMax.Value;
            _zMin = GeoPoint.ZMin.Value;
            _zMax = GeoPoint.ZMax.Value;

            _Dx = _xMax - _xMin;
            _Dy = _yMax - _yMin;
            _Dz = _zMax - _zMin;

            double d = _Dx;
            if (_Dy > d) d = _Dy;
            if (_Dz > d) d = _Dz;

            _nX = (int)Math.Ceiling(50.0 * _Dx / d);
            _nY = (int)Math.Ceiling(50.0 * _Dy / d);
            _nZ = (int)Math.Ceiling(50.0 * _Dz / d);

            foreach (SuperposerSegment segment in _segmentList)
            {
                Dictionary<int, SuperposerArea> areaDictionary = new Dictionary<int, SuperposerArea>();
                getArea(segment.A1, areaDictionary);
                getArea(segment.A2, areaDictionary);

                foreach (SuperposerArea area in areaDictionary.Values) area.addSegment(segment);
            }
        }

        internal void superposeLine(List<GeoPoint> pointList)
        {
            int i, n = pointList.Count;
            for (i = 1; i < n - 1; ++i)
            {
                GeoPoint nearestPoint = getNearestPoint(pointList[i]);
                if (nearestPoint != null) pointList[i] = nearestPoint;
                if (DistanceCalculator.getDistance(pointList[i - 1], pointList[i]) < 0.0001)
                {
                    pointList.RemoveAt(i);
                    --i;
                    --n;
                }
            }
        }

        private void getArea(GeoPoint point, Dictionary<int, SuperposerArea> areaDictionary)
        {
            int i, j, k;
            double x, y, z;
            for (i = -1; i <= 1; ++i)
            {
                x = point.X + (double)i * _threshold;
                for (j = -1; j <= 1; ++j)
                {
                    y = point.Y + (double)j * _threshold;
                    for (k = -1; k <= 1; ++k)
                    {
                        z = point.Z + (double)k * _threshold;
                        int index = getIndex(x, y, z);

                        SuperposerArea area = null;
                        if (!_areaDictionary.TryGetValue(index, out area))
                        {
                            area = new SuperposerArea();
                            _areaDictionary.Add(index, area);
                        }

                        if (!areaDictionary.ContainsKey(index)) areaDictionary.Add(index, area);
                    }
                }
            }
        }

        private int getIndex(double x, double y, double z)
        {
            int iX = (int)Math.Floor((double)_nX * (x - _xMin) / (_xMax - _xMin));
            int iY = (int)Math.Floor((double)_nY * (y - _yMin) / (_yMax - _yMin));
            int iZ = (int)Math.Floor((double)_nZ * (z - _zMin) / (_zMax - _zMin));

            if (iX < 0) iX = 0;
            if (iX >= _nX) iX = _nX - 1;
            if (iY < 0) iY = 0;
            if (iY >= _nX) iY = _nY - 1;
            if (iZ < 0) iZ = 0;
            if (iZ >= _nZ) iZ = _nZ - 1;

            return _nZ * (iX * _nY + iY) + iZ;
        }

        private GeoPoint getNearestPoint(GeoPoint point)
        {
            int index = getIndex(point.X, point.Y, point.Z);

            SuperposerArea area = null;
            if (!_areaDictionary.TryGetValue(index, out area)) return null;

            return area.getNearestPoint(point, _threshold);
        }
    }
}
