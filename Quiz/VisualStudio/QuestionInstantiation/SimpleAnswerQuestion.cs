using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace QuestionInstantiation
{
    class SimpleAnswerQuestion
    {
        private readonly Text _questionText;
        private readonly Choice _choice;
        private readonly Choice _excludedChoice;
        private readonly Element _questionElement;
        private readonly XmlSimpleAnswerProximityCriterionEnum _proximityCriterion;

        internal SimpleAnswerQuestion(Text questionText, Choice choice, Choice excludedChoice, Element questionElement, XmlSimpleAnswerProximityCriterionEnum proximityCriterion)
        {
            _questionText = questionText;
            _choice = choice;
            _excludedChoice = excludedChoice;
            _questionElement = questionElement;
            _proximityCriterion = proximityCriterion;
        }

        internal Text QuestionText { get { return _questionText; } }
        internal Choice Choice { get { return _choice; } }
        internal Element QuestionElement { get { return _questionElement; } }
        internal Choice ExcludedChoice { get { return _excludedChoice; } }
        internal XmlSimpleAnswerProximityCriterionEnum ProximityCriterion { get { return _proximityCriterion; } }
    }
}
