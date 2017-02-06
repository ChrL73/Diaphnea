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
    }
}
