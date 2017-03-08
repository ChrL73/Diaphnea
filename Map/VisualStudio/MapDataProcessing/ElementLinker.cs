using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class ElementLinker
    {
        internal XmlResolution Resolution { get; set; }
        internal XmlProjectionEnum Projection { get; set; }

        private double _yMin = Double.MaxValue;
        private double _yMax = Double.MinValue;

        private readonly List<LinkerSegment> _segmentList = new List<LinkerSegment>();

        internal int addPolygon(PolygonMapElement element)
        {
            List<GeoPoint> contour = element.getContourLine(Resolution);

            int i, n = contour.Count;
            for (i = 0; i < n - 1; ++i)
            {
                double x1, y1, x2, y2;
                if (!contour[i].getProjection(Projection, out x1, out y1))
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, "Projection error");
                    return -1;
                }

                if (!contour[i + 1].getProjection(Projection, out x2, out y2))
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, "Projection error");
                    return -1;
                }

                LinkerSegment segment = new LinkerSegment(element, x1, y1, x2, y2);

                if (i == 0)
                {
                    if (y1 < _yMin) _yMin = y1;
                    if (y1 > _yMax) _yMax = y1;
                }

                if (y2 < _yMin) _yMin = y2;
                if (y2 > _yMax) _yMax = y2;
            }
            
            return 0;
        }

        internal void classifySegments()
        {

        }
    }
}
