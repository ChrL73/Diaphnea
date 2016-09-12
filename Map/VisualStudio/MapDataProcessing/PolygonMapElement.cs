using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class PolygonMapElement : MapElement
    {
        private readonly List<String> _lineKmlFileList = new List<String>();
        private readonly List<String> _pointKmlFileList = new List<String>();

        internal PolygonMapElement(String id) : base(id) { }

        internal override void addKmlFile(String path)
        {

        }
    }
}
