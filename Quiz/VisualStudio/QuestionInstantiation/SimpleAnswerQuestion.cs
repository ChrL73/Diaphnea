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
        private readonly Element _element;
        private readonly PossibleAnswer _answer;

        internal SimpleAnswerQuestion(Text questionText, PossibleAnswer answer, Element element)
        {
            _questionText = questionText;
            _element = element;
            _answer = answer;
        }

        internal BsonDocument getBsonDocument()
        {
            BsonDocument questionDocument = new BsonDocument()
            {
                { "question_text", _questionText.getBsonDocument() },
                { "answer", _answer.getBsonDocument() }
            };

            return questionDocument;
        }

    }
}
