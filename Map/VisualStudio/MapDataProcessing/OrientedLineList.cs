using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class OrientedLineList
    {
        private readonly List<OrientedPolygonLinePart> _lineList = new List<OrientedPolygonLinePart>();

        internal List<OrientedPolygonLinePart> LineList { get { return _lineList; } }
    }
}
