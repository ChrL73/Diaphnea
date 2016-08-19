using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class MultipleAnswerQuestion
    {
        private readonly Text _questionText;
        private readonly Dictionary<Choice, int> _choiceDictionary = new Dictionary<Choice, int>();
        private readonly List<Choice> _choiceList = new List<Choice>();
        private readonly Choice _excludedChoice;
        //private readonly Element _element;
        private readonly XmlAnswerProximityCriterionEnum _proximityCriterion;

        internal MultipleAnswerQuestion(Text questionText, Choice excludedChoice/*, Element element*/, XmlAnswerProximityCriterionEnum proximityCriterion)
        {
            _questionText = questionText;
            _excludedChoice = excludedChoice;
            //_element = element;
            _proximityCriterion = proximityCriterion;
        }

        internal void addChoice(Choice choice)
        {
            _choiceDictionary.Add(choice, 0);
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
