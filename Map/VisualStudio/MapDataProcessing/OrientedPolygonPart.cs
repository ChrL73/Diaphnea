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

        internal KmlFileData Line { get { return _part.Line; } }
        internal KmlFileData Point1 { get { return _orientation == OrientationEnum.DIRECT ? _part.Point1 : _part.Point2; } }
        internal KmlFileData Point2 { get { return _orientation == OrientationEnum.DIRECT ? _part.Point2 : _part.Point1; } }
    }
}
