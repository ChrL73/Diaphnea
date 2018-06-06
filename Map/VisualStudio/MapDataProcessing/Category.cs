using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace MapDataProcessing
{
    class Category
    {
        private static int _counter = -1;
        internal static void reset() { _counter = -1; }

        private readonly int _id;
        internal int Id { get { return _id; } }

        private readonly XmlCategory _xmlCategory;
        //private readonly ElementName _name;

        internal XmlCategory XmlCategory { get { return _xmlCategory; } }

        internal Category(XmlCategory xmlCategory)
        {
            _id = ++_counter;
            _xmlCategory = xmlCategory;
            //_name = new ElementName(xmlCategory.name);
        }

        internal double Importance { get { return _xmlCategory.importance; } }
    }
}
