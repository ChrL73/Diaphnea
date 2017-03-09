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

        internal LinkerSegment(PolygonMapElement element, double x1, double y1, double x2, double y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            _element = element;
        }

        internal PolygonMapElement Element { get { return _element; } }
        internal double X1 { get; set; }
        internal double Y1 { get; set; }
        internal double X2 { get; set; }
        internal double Y2 { get; set; }
    }
}
