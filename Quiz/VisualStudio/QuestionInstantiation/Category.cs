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

        protected Category(int weightIndex, string questionNameInLog)
        {
            _weightIndex = weightIndex;
            _questionNameInLog = questionNameInLog;
        }

        internal Int32 WeightIndex
        {
            get { return _weightIndex; }
        }

        protected string QuestionNameInLog
        {
            get { return _questionNameInLog; }
        }

        abstract internal BsonDocument getBsonDocument(IMongoDatabase database, string questionnaireId);
    }
}
