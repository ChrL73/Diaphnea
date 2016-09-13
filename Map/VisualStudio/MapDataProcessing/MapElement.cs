using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    abstract class MapElement
    {
        private readonly String _id;

        internal MapElement(String id)
        {
            _id = id;
        }

        abstract internal int addKmlFile(String path);

        internal String Id { get { return _id; } }
    }
}
