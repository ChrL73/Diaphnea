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

namespace QuestionInstantiation
{
    class GeoPoint
    {
        private readonly double _x;
        private readonly double _y;
        private readonly double _z;

        internal GeoPoint(double longitude, double latitude)
        {
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
        }

        static internal GeoPoint meanGeoPoint(IEnumerable<GeoPoint> geoPoints)
        {
            double x = 0.0, y = 0.0, z = 0.0;

            foreach(GeoPoint point in geoPoints)
            {
                x += point._x;
                y += point._y;
                z += point._z;
            }

            double invN = 1.0 / (double)geoPoints.Count();
            x *= invN;
            y *= invN;
            z *= invN;

            return new GeoPoint(x, y, z);
        }

        internal double X { get { return _x; } }
        internal double Y { get { return _y; } }
        internal double Z { get { return _z; } }
    }
}
