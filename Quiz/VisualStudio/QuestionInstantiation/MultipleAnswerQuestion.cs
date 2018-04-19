using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace QuestionInstantiation
{
    class MultipleAnswerQuestion
    {
        private readonly Text _questionText;
        private readonly Dictionary<Choice, int> _choiceDictionary = new Dictionary<Choice, int>();
        private readonly List<Choice> _choiceList = new List<Choice>();
        private readonly Choice _excludedChoice;
        private readonly Element _questionElement;
        //private readonly XmlMultipleAnswerProximityCriterionEnum _proximityCriterion;
        private readonly ChoiceCommentModeEnum _choiceCommentMode;

        internal MultipleAnswerQuestion(Text questionText, Choice excludedChoice, Element questionElement, /*XmlMultipleAnswerProximityCriterionEnum proximityCriterion,*/
                                        ChoiceCommentModeEnum choiceCommentMode)
        {
            _questionText = questionText;
            _excludedChoice = excludedChoice;
            _questionElement = questionElement;
            //_proximityCriterion = proximityCriterion;
            _choiceCommentMode = choiceCommentMode;
        }

        internal Text QuestionText { get { return _questionText; } }
        internal Element QuestionElement { get { return _questionElement; } }
        internal List<Choice> ChoiceList { get { return _choiceList; } }
        internal ChoiceCommentModeEnum ChoiceCommentMode { get { return _choiceCommentMode; } }
        internal Choice ExcludedChoice { get { return _excludedChoice; } }

        internal void addChoice(Choice choice)
        {
            _choiceDictionary[choice] = 0;
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
    }
}
