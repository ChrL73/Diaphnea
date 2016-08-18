using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    abstract class Category
    {
        private readonly Int32 _weightIndex;
        private readonly string _questionNameInLog;
        private readonly QuizData _quizData;

        protected Category(int weightIndex, string questionNameInLog, QuizData quizData)
        {
            _weightIndex = weightIndex;
            _questionNameInLog = questionNameInLog;
            _quizData = quizData;
        }

        protected Int32 WeightIndex
        {
            get { return _weightIndex; }
        }

        protected string QuestionNameInLog
        {
            get { return _questionNameInLog; }
        }

        protected QuizData QuizData
        {
            get { return _quizData; }
        }

        abstract internal BsonDocument getBsonDocument(IMongoDatabase database, string questionnaireId);
    }
}
