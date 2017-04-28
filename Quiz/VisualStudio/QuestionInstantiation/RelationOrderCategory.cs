using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class RelationOrderCategory : Category
    {
        private readonly List<RelationOrderQuestion> _questionList = new List<RelationOrderQuestion>();
        private readonly double _distribParameterCorrection;

        internal RelationOrderCategory(int weightIndex, string questionNameInLog, QuizData quizData, double distribParameterCorrection)
            : base(weightIndex, questionNameInLog, quizData)
        {
            _distribParameterCorrection = distribParameterCorrection;
        }

        internal void addQuestion(RelationOrderQuestion question)
        {
            _questionList.Add(question);
        }

        internal int QuestionCount
        {
            get { return _questionList.Count; }
        }

        internal override BsonDocument getBsonDocument(IMongoDatabase database, string questionnaireId)
        {
            IMongoCollection<BsonDocument> questionListsCollection = database.GetCollection<BsonDocument>("question_lists");
            BsonDocument questionListDocument = getQuestionListDocument(database, questionnaireId);
            questionListsCollection.InsertOne(questionListDocument);

            BsonDocument categoryDocument = new BsonDocument()
            {
                { "type", "RelationOrder" },
                { "question_count", _questionList.Count },
                { "question_list", questionListDocument.GetValue("_id") },
                { "weight_index", WeightIndex },
                { "distrib_parameter_correction", _distribParameterCorrection }
            };

            return categoryDocument;
        }

        internal BsonDocument getQuestionListDocument(IMongoDatabase database, string questionnaireId)
        {
            BsonDocument questionListDocument = new BsonDocument();

            BsonArray questionsArray = new BsonArray();
            foreach (RelationOrderQuestion question in _questionList)
            {
                questionsArray.Add(question.getBsonDocument(database, questionnaireId, QuizData));
            }

            BsonDocument questionsDocument = new BsonDocument()
            {
                { "questionnaire", questionnaireId },
                { "count", _questionList.Count },
                { "questions", questionsArray }
            };

            questionListDocument.AddRange(questionsDocument);

            return questionListDocument;
        }
    }
}
