using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class LinkerSegment
    {
        private readonly PolygonMapElement _element;
        private readonly double _x1;
        private readonly double _y1;
        private readonly double _x2;
        private readonly double _y2;

        internal LinkerSegment(PolygonMapElement element, double x1, double y1, double x2, double y2)
        {
            _x1 = x1;
            _y1 = y1;
            _x2 = x2;
            _y2 = y2;
            _element = element;
        }

        internal PolygonMapElement Element { get { return _element; } }
    }
}
