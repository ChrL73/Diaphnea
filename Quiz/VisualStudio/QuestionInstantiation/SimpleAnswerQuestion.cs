using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class SimpleAnswerQuestion
    {
        private readonly string _questionText;
        private readonly Element _element;
        private readonly PossibleAnswer _answer;

        internal SimpleAnswerQuestion(string questionText, PossibleAnswer answer, Element element)
        {
            _questionText = questionText;
            _element = element;
            _answer = answer;
        }

    }
}
