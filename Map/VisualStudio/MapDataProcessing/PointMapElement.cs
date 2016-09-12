using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class PointMapElement : MapElement
    {
        private String _KmlFile;

        internal PointMapElement(String id) : base(id) { }

        internal override void addKmlFile(String path)
        {

        }
    }
}
