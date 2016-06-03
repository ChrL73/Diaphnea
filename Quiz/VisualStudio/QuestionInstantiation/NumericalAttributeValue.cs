using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class NumericalAttributeValue
    {
        private readonly double _value;
        private readonly XmlNumericalAttributeType _numericalAttributeType;

        internal NumericalAttributeValue(double value, XmlNumericalAttributeType numericalAttributeType)
        {
            _value = value;
            _numericalAttributeType = numericalAttributeType;
        }

        internal double Value
        {
            get { return _value; }
        }
    }
}
