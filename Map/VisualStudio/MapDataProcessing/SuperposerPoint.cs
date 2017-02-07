using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class SuperposerPoint
    {
        private readonly List<GeoPoint> _line;
        private readonly int _i;

        internal SuperposerPoint(List<GeoPoint> line, int i)
        {
            _line = line;
            _i = i;
        }

        internal List<GeoPoint> Line { get { return _line; } }
        internal int I { get { return _i; } }
    }
}
