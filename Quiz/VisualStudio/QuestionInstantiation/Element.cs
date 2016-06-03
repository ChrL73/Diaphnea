using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class Element
    {
        private XmlElement _xmlElement;

        internal Element(XmlElement xmlElement)
        {
            _xmlElement = xmlElement;
        }
    }
}
