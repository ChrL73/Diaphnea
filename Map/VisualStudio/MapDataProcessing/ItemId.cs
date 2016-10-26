using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class ItemId
    {
        private static int _counter = -1;
        internal static void reset() { _counter = -1; }

        private readonly int _value;
        internal ItemId() { _value = ++_counter; }
        internal int Value { get { return _value; } }
    }
}
