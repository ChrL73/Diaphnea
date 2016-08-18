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
        private readonly Choice _choice;
        private readonly Choice _excludedChoice;
        //private readonly Element _element;
        private readonly XmlAnswerProximityCriterionEnum _proximityCriterion;

        internal SimpleAnswerQuestion(Text questionText, Choice choice, Choice excludedChoice/*, Element element*/, XmlAnswerProximityCriterionEnum proximityCriterion)
        {
            _questionText = questionText;
            _choice = choice;
            _excludedChoice = excludedChoice;
            //_element = element;
            _proximityCriterion = proximityCriterion;
        }

        internal BsonDocument getBsonDocument(QuizData quizData)
        {
            BsonDocument questionDocument = new BsonDocument()
            {
                { "question", _questionText.getBsonDocument() }
            };

            questionDocument.AddRange(_choice.getBsonDocument());

            if (_excludedChoice != null)
            {
                questionDocument.AddRange(new BsonDocument() { { "excluded_choice", _excludedChoice.AttributeValue.Value.getBsonDocument() } });
            }
            else
            {
                questionDocument.AddRange(new BsonDocument() { { "excluded_choice", Text.emptyText(quizData).getBsonDocument() } });
            }

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
            else if (_proximityCriterion == XmlAnswerProximityCriterionEnum.ELEMENT_LOCATION)
            {
                questionDocument.AddRange(new BsonDocument()
                {
                    { "proximity_criterion_value", _choice.Element.GeoPoint.getBsonDocument() }
                });
            }

            return questionDocument;
        }

    }
}
