using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class AttributeValue
    {
        private readonly string _value;
        private readonly string _comment;
        private readonly XmlAttributeType _attributeType;

        internal AttributeValue(string value, string comment, XmlAttributeType attributeType)
        {
            _value = value;
            _comment = comment;
            _attributeType = attributeType;
        }

        internal string Value
        {
            get { return _value; }
        }

        internal string Comment
        {
            get { return _comment; }
        }
    }
}
