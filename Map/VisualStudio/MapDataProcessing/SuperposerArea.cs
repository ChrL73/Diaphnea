using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class SuperposerArea
    {
        private readonly List<SuperposerSegment> _segmentList = new List<SuperposerSegment>();

        internal void addSegment(SuperposerSegment segment) { _segmentList.Add(segment); }

        internal SuperposerPoint getNearestPoint(GeoPoint point, double threshold)
        {
            double dMin = threshold;
            int bestI = -1;

            int i, n = _segmentList.Count();
            for (i = 0; i < n; ++i)
            {
                double d = _segmentList[i].getDistance(point);
                if (d < dMin)
                {
                    dMin = d;
                    bestI = i;
                }
            }

            if (bestI != -1) return _segmentList[bestI].getNearestExtremity(point);
            return null;
        }
    }
}
