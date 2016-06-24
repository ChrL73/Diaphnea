using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class AttributeValue
    {
        private readonly Text _value;
        private readonly Text _comment;
        private readonly XmlAttributeType _attributeType;

        internal AttributeValue(Text value, Text comment, XmlAttributeType attributeType)
        {
            _value = value;
            _comment = comment;
            _attributeType = attributeType;
        }

        internal Text Value
        {
            get { return _value; }
        }

        internal Text Comment
        {
            get { return _comment; }
        }
    }
}
