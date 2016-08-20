using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class MultipleAnswerQuestion
    {
        private readonly Text _questionText;
        private readonly Dictionary<Choice, int> _choiceDictionary = new Dictionary<Choice, int>();
        private readonly List<Choice> _choiceList = new List<Choice>();
        private readonly Choice _excludedChoice;
        //private readonly Element _element;
        private readonly XmlMultipleAnswerProximityCriterionEnum _proximityCriterion;

        internal MultipleAnswerQuestion(Text questionText, Choice excludedChoice/*, Element element*/, XmlMultipleAnswerProximityCriterionEnum proximityCriterion)
        {
            _questionText = questionText;
            _excludedChoice = excludedChoice;
            //_element = element;
            _proximityCriterion = proximityCriterion;
        }

        internal void addChoice(Choice choice)
        {
            _choiceDictionary.Add(choice, 0);
            _choiceList.Add(choice);
        }

        internal int getChoiceCount()
        {
            return _choiceList.Count;
        }

        internal Choice getChoice(int i)
        {
            return _choiceList.ElementAt(i);
        }

        internal BsonDocument getBsonDocument(QuizData quizData)
        {
            BsonDocument questionDocument = new BsonDocument()
            {
                { "question", _questionText.getBsonDocument() }
            };

            BsonArray choiceArray = new BsonArray();
            foreach (Choice choice in _choiceList) choiceArray.Add(choice.getBsonDocument());
            questionDocument.AddRange(new BsonDocument() { { "answers", choiceArray } });

            if (_excludedChoice != null)
            {
                questionDocument.AddRange(new BsonDocument() { { "excluded_choice", _excludedChoice.AttributeValue.Value.getBsonDocument() } });
            }
            else
            {
                questionDocument.AddRange(new BsonDocument() { { "excluded_choice", Text.emptyText(quizData).getBsonDocument() } });
            }

            if (_proximityCriterion == XmlMultipleAnswerProximityCriterionEnum.ELEMENT_LOCATION)
            {
                List<GeoPoint> pointList = new List<GeoPoint>();
                foreach (Choice choice in _choiceList) pointList.Add(choice.Element.GeoPoint);
                GeoPoint meanGeoPoint = GeoPoint.meanGeoPoint(pointList);
                questionDocument.AddRange(new BsonDocument()
                {
                    { "proximity_criterion_value", meanGeoPoint.getBsonDocument() }
                });
            }

            return questionDocument;
        }
    }
}
