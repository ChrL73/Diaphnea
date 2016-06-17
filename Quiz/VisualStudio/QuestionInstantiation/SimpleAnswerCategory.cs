using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class SimpleAnswerCategory : Category
    {
        private readonly List<PossibleAnswer> _answerList = new List<PossibleAnswer>();
        private readonly Dictionary<string, List<PossibleAnswer>> _answerDictionary = new Dictionary<string, List<PossibleAnswer>>();
        private readonly List<SimpleAnswerQuestion> _questionList = new List<SimpleAnswerQuestion>();
        private readonly XmlAnswerProximityCriterionEnum _proximityCriterion;
        private readonly double _distribParameterCorrection;

        internal SimpleAnswerCategory(Int32 weightIndex, XmlAnswerProximityCriterionEnum proximityCriterion, double distribParameterCorrection) : base(weightIndex)
        {
            _proximityCriterion = proximityCriterion;
            _distribParameterCorrection = distribParameterCorrection;
        }

        internal int QuestionCount
        {
            get { return _questionList.Count; }
        }

        internal int DistinctAnswerCount
        {
            get { return _answerDictionary.Count; }
        }

        internal int TotalAnswerCount
        {
            get { return _answerList.Count; }
        }

        internal void addAnswer(PossibleAnswer answer)
        {
            _answerList.Add(answer);
            string answerString = answer.AttributeValue.Value;
            if (!_answerDictionary.ContainsKey(answerString)) _answerDictionary.Add(answerString, new List<PossibleAnswer>());
            _answerDictionary[answerString].Add(answer);
        }

        internal void addQuestion(SimpleAnswerQuestion question)
        {
            _questionList.Add(question);
        }

        internal void setComments(XmlAttributeType attributeType)
        {
            foreach (List<PossibleAnswer> answerList in _answerDictionary.Values)
            {
                List<string> list = new List<string>();
                foreach (PossibleAnswer answer in answerList)
                {
                    if (attributeType == null)
                    {
                        list.Add(answer.Element.XmlElement.name);
                    }
                    else
                    {
                        AttributeValue attributeValue = answer.Element.getAttributeValue(attributeType);
                        if (attributeValue != null) list.Add(attributeValue.Value);
                    }
                }

                string comment = String.Join(", ", list);
                foreach (PossibleAnswer answer in answerList) answer.Comment = comment;
            }
        }

        internal override BsonDocument getBsonDocument()
        {
            BsonDocument categoryDocument = new BsonDocument()
            {
                { "just_a_test", _questionList.Count }
            };

            return categoryDocument;
        }
    }
}
