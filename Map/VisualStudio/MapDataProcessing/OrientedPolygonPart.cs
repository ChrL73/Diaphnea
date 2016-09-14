using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class OrientedPolygonPart
    {
        private readonly PolygonPart _part;
        private readonly OrientationEnum _orientation;

        internal OrientedPolygonPart(PolygonPart part, OrientationEnum orientation)
        {
            _part = part;
            _orientation = orientation;
        }

        internal PolygonPart Part { get { return _part; } }
        internal OrientationEnum Orientation { get { return _orientation; } }
    }
}
