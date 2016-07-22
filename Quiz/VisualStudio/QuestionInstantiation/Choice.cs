using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class Choice
    {
        private readonly AttributeValue _attributeValue;
        private readonly Element _element;

        internal Choice(AttributeValue attributeValue, Element element)
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

        internal BsonDocument getBsonDocument()
        {
            BsonDocument choiceDocument = new BsonDocument()
            {
                { "answer", _attributeValue.Value.getBsonDocument() },
                { "comment", _attributeValue.Comment.getBsonDocument() }
            };

            return choiceDocument;
        }
    }
}
