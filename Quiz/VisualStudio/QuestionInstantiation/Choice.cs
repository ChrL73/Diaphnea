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

        internal Choice(AttributeValue attributeValue, Element element, QuizData quizData)
        {
            _attributeValue = attributeValue;
            _element = element;
            Comment = Text.emptyText(quizData);
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

        internal BsonDocument getBsonDocument(ChoiceCommentModeEnum commentMode)
        {
            BsonDocument choiceDocument = new BsonDocument()
            {
                { "answer", _attributeValue.Value.getBsonDocument() },
                { "comment", (commentMode == ChoiceCommentModeEnum.ATTRIBUTE_COMMENT) ? _attributeValue.Comment.getBsonDocument() : Comment.getBsonDocument() },
                { "answer_map_id", Element.XmlElement.mapId == null ? "" : Element.XmlElement.mapId.Substring(2) }
            };

            return choiceDocument;
        }
    }
}
