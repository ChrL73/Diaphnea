using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class SimpleAnswerQuestion
    {
        private readonly Text _questionText;
        //private readonly Element _element;
        private readonly Choice _choice;
        private readonly XmlAnswerProximityCriterionEnum _proximityCriterion;

        internal SimpleAnswerQuestion(Text questionText, Choice choice/*, Element element*/, XmlAnswerProximityCriterionEnum proximityCriterion)
        {
            _questionText = questionText;
            //_element = element;
            _choice = choice;
            _proximityCriterion = proximityCriterion;
        }

        internal BsonDocument getBsonDocument()
        {
            BsonDocument questionDocument = new BsonDocument()
            {
                { "question", _questionText.getBsonDocument() }
            };

            questionDocument.AddRange(_choice.getBsonDocument());

            if (_proximityCriterion == XmlAnswerProximityCriterionEnum.SORT_KEY)
            {
                questionDocument.AddRange(new BsonDocument()
                {
                    { "proximity_criterion_value", _choice.Element.XmlElement.sortKey }
                });
            }
            else if (_proximityCriterion == XmlAnswerProximityCriterionEnum.ATTRIBUTE_VALUE_AS_NUMBER)
            {
                Double? d = _choice.AttributeValue.Value.getAsDouble();
                if (d == null) d = 0.0;
                questionDocument.AddRange(new BsonDocument()
                {
                    { "proximity_criterion_value", d }
                });
            }
            else
            {
                throw new NotImplementedException();
            }

            return questionDocument;
        }

    }
}
