using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapColors
{
    class Department
    {
        internal string Id { get; set; }
        internal Region Region { get; set; }
        internal Dictionary<Department, int> AdjacentDepartments { get; set; }
        internal double Latitude { get; set; }
        internal double Longitude { get; set; }
        internal ColorEnum Color { get; set; }

        internal Department()
        {
            Color = ColorEnum.NONE;
            AdjacentDepartments = new Dictionary<Department, int>();
        }

        internal double distance(Department department)
        {
            double d = 0.0;

            if (department != this)
            {
                if (department.Region != Region) d += 1e8;
                if (!AdjacentDepartments.ContainsKey(department)) d += 1e4;

                double d1 = Latitude - department.Latitude;
                double d2 = Longitude - department.Longitude;
                d += d1 * d1 + d2 * d2;
            }

            return d;
        }
    }
}
