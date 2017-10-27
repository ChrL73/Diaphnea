using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapColors
{
    class Region
    {
        internal string Id { get; set; }
        internal List<Department> Departments { get; set; }

        internal Region()
        {
            Departments = new List<Department>();
        }
    }
}
