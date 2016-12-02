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
    }
}
