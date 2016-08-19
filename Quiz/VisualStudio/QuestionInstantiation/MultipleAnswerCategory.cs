using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class MultipleAnswerCategory : Category
    {
        private readonly SortedDictionary<Text, List<Choice>> _choiceDictionary = new SortedDictionary<Text, List<Choice>>(new TextComparer());
        private readonly List<MultipleAnswerQuestion> _questionList = new List<MultipleAnswerQuestion>();
        private readonly XmlAnswerProximityCriterionEnum _proximityCriterion;
        private readonly double _distribParameterCorrection;

        internal MultipleAnswerCategory(Int32 weightIndex, string questionNameInLog, QuizData quizData, XmlAnswerProximityCriterionEnum proximityCriterion, double distribParameterCorrection)
            : base(weightIndex, questionNameInLog, quizData)
        {
            _proximityCriterion = proximityCriterion;
            _distribParameterCorrection = distribParameterCorrection;
        }

        internal int QuestionCount
        {
            get { return _questionList.Count; }
        }

        internal int ChoiceCount
        {
            get { return _choiceDictionary.Count; }
        }

        internal void addChoice(Choice choice)
        {
            Text choiceText = choice.AttributeValue.Value;
            if (!_choiceDictionary.ContainsKey(choiceText)) _choiceDictionary.Add(choiceText, new List<Choice>());
            _choiceDictionary[choiceText].Add(choice);
        }

        internal void addQuestion(MultipleAnswerQuestion question)
        {
            _questionList.Add(question);
        }

        internal void setComments(RelationType relationType, XmlAttributeType attributeType)
        {
            throw new NotImplementedException();
        }

        internal void setComments(RelationType relationType, RelationType relation2Type, XmlAttributeType attributeType)
        {
            throw new NotImplementedException();
        }

        internal override BsonDocument getBsonDocument(IMongoDatabase database, string questionnaireId)
        {
            return null;
        }
    }
}
