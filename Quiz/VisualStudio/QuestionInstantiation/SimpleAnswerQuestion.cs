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
        private readonly Choice _choice;

        internal SimpleAnswerQuestion(Text questionText, Choice choice, Element element)
        {
            _questionText = questionText;
            _element = element;
            _choice = choice;
        }

        internal BsonDocument getBsonDocument()
        {
            BsonDocument questionDocument = new BsonDocument()
            {
                { "question", _questionText.getBsonDocument() }
            };

            questionDocument.AddRange(_choice.getBsonDocument());

            return questionDocument;
        }

    }
}
