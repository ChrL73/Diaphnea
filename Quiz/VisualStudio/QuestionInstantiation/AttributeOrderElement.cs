using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class AttributeOrderElement
    {
        private readonly double _attributeValue;
        private readonly Element _element;

        internal AttributeOrderElement(double attributeValue, Element element)
        {
            _attributeValue = attributeValue;
            _element = element;
            MinAnswerIndex = -1;
        }

        internal int MinAnswerIndex { get; set; }
        internal double AttributeValue { get { return _attributeValue; } }
        internal Element Element { get { return _element; } }
    }
}
