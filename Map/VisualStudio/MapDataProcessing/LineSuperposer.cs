/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

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
            for (i = 0; i < n - 1; ++i) _segmentList.Add(new SuperposerSegment(pointList, i));
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
            List<GeoPoint> currentLine = null;
            int i1 = -1;
            int i2 = -1;
            int j1 = -1;
            int j2 = -1;

            int j, n = pointList.Count;
            for (j = 1; j < n - 1; ++j)
            {
                SuperposerPoint nearestPoint = getNearestPoint(pointList[j]);

                if (nearestPoint != null)
                {
                    if (currentLine == null)
                    {
                        currentLine = nearestPoint.Line;
                        i1 = nearestPoint.I;
                        i2 = nearestPoint.I;
                        j1 = j;
                        j2 = j;
                    }
                    else
                    {
                        if (nearestPoint.Line == currentLine)
                        {
                            i2 = nearestPoint.I;
                            j2 = j;
                        }
                        else
                        {
                            replaceLine(pointList, j1, j2, currentLine, i1, i2);
                            j += pointList.Count - n;
                            n = pointList.Count;

                            currentLine = nearestPoint.Line;
                            i1 = nearestPoint.I;
                            i2 = nearestPoint.I;
                            j1 = j;
                            j2 = j;
                        }
                    }
                }
                else
                {
                    if (currentLine != null && j2 > j - 1)
                    {
                        replaceLine(pointList, j1, j2, currentLine, i1, i2);
                        j += pointList.Count - n;
                        n = pointList.Count;
                    }

                    currentLine = null;
                }
            }

            n = pointList.Count;
            for (j = 0; j < n - 1; ++j)
            {
                if (DistanceCalculator.getDistance(pointList[j], pointList[j + 1]) < 0.00001)
                {
                    pointList.RemoveAt(j);
                    --j;
                    --n;
                }
            }
        }

        void replaceLine(List<GeoPoint> lineJ, int j1, int j2, List<GeoPoint> lineI, int i1, int i2)
        {
            if (i1 == i2 || i1 == i2 - 1 || i1 == i2 + 1 || j1 == j2 || j1 == j2 - 1) return;

            List<GeoPoint> line = new List<GeoPoint>();
            line.Add(new GeoPoint(0.5 * (lineI[i1].X + lineJ[j1].X), 0.5 * (lineI[i1].Y + lineJ[j1].Y), 0.5 * (lineI[i1].Z + lineJ[j1].Z)));

            int i;
            if (i2 > i1) for (i = i1 + 1; i <= i2 - 1; ++i) line.Add(lineI[i]);
            for (i = i1 - 1; i >= i2 + 1; --i) line.Add(lineI[i]);

            line.Add(new GeoPoint(0.5 * (lineI[i2].X + lineJ[j2].X), 0.5 * (lineI[i2].Y + lineJ[j2].Y), 0.5 * (lineI[i2].Z + lineJ[j2].Z)));

            lineJ.RemoveRange(j1, j2 - j1 + 1);
            lineJ.InsertRange(j1, line);
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
            if (iY >= _nY) iY = _nY - 1;
            if (iZ < 0) iZ = 0;
            if (iZ >= _nZ) iZ = _nZ - 1;

            return _nZ * (iX * _nY + iY) + iZ;
        }

        private SuperposerPoint getNearestPoint(GeoPoint point)
        {
            int index = getIndex(point.X, point.Y, point.Z);

            SuperposerArea area = null;
            if (!_areaDictionary.TryGetValue(index, out area)) return null;

            return area.getNearestPoint(point, _threshold);
        }
    }
}
