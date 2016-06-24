using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class PossibleAnswer
    {
        private readonly AttributeValue _attributeValue;
        private readonly Element _element;

        internal PossibleAnswer(AttributeValue attributeValue, Element element)
        {
            _attributeValue = attributeValue;
            _element = element;
        }

        internal Text Comment { get; set; }

        internal AttributeValue AttributeValue
        {
            get { return _attributeValue; }
        }

        internal Element Element
        {
            get { return _element; }
        }
    }
}
