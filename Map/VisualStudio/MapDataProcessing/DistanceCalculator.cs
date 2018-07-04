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
    class DistanceCalculator
    {
        // Distance between a polyline and a point
        static internal double getDistance(List<GeoPoint> line, GeoPoint point)
        {
            int n = line.Count;
            if (n == 0) return Double.MaxValue;
            if (n == 1) return getDistance(line[0], point);

            double dMin = Double.MaxValue;
            int i;
            for (i = 1; i < n; ++i)
            {
                double d = getDistance(line[i - 1], line[i], point);
                if (d < dMin) dMin = d;
            }

            return dMin;
        }

        // Distance between segment [A1, A2] and point B
        static internal double getDistance(GeoPoint A1, GeoPoint A2, GeoPoint B)
        {
            double ux = A2.X - A1.X;
            double uy = A2.Y - A1.Y;
            double uz = A2.Z - A1.Z;
            double r = Math.Sqrt(ux * ux + uy * uy + uz * uz);
            ux /= r;
            uy /= r;
            uz /= r;

            double vx = B.X - A1.X;
            double vy = B.Y - A1.Y;
            double vz = B.Z - A1.Z;
            double a = vx * ux + vy * uy + vz * uz;

            if (a <= 0) return getDistance(A1, B);
            if (a >= r) return getDistance(A2, B);

            GeoPoint C = new GeoPoint(A1.X + a * ux, A1.Y + a * uy, A1.Z + a * uz);
            return getDistance(C, B);
        }

        // Distance between point A and point B
        static internal double getDistance(GeoPoint A, GeoPoint B)
        {
            double dx = B.X - A.X;
            double dy = B.Y - A.Y;
            double dz = B.Z - A.Z;

            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        static internal GeoPoint getNearestPoint(List<GeoPoint> line, GeoPoint point)
        {
            int n = line.Count;
            if (n == 0) return null;
            if (n == 1) return line[0];

            double dMin = Double.MaxValue;
            int i, i0 = 1;
            for (i = 1; i < n; ++i)
            {
                double d = getDistance(line[i - 1], line[i], point);
                if (d < dMin)
                {
                    dMin = d;
                    i0 = i;
                }
            }

            return getNearestPoint(line[i0 - 1], line[i0], point);
        }

        static internal GeoPoint getNearestPoint(GeoPoint A1, GeoPoint A2, GeoPoint B)
        {
            double ux = A2.X - A1.X;
            double uy = A2.Y - A1.Y;
            double uz = A2.Z - A1.Z;
            double r = Math.Sqrt(ux * ux + uy * uy + uz * uz);
            ux /= r;
            uy /= r;
            uz /= r;

            double vx = B.X - A1.X;
            double vy = B.Y - A1.Y;
            double vz = B.Z - A1.Z;
            double a = vx * ux + vy * uy + vz * uz;

            if (a <= 0) return A1;
            if (a >= r) return A2;

            return new GeoPoint(A1.X + a * ux, A1.Y + a * uy, A1.Z + a * uz);
        }
    }
}
