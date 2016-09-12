using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class LineMapElement : MapElement
    {
        private readonly List<String> _KmlFileList = new List<String>();

        internal LineMapElement(String id) : base(id) { }

        internal override void addKmlFile(String path)
        {

        }
    }
}
