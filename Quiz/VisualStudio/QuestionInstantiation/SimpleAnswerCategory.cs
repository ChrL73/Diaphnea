using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class SimpleAnswerCategory : Category
    {
        private readonly List<PossibleAnswer> _answerList = new List<PossibleAnswer>();
        private readonly Dictionary<string, List<PossibleAnswer>> _answerDictionary = new Dictionary<string, List<PossibleAnswer>>();
        private readonly List<SimpleAnswerQuestion> _questionList = new List<SimpleAnswerQuestion>();
        private readonly XmlAnswerProximityCriterionEnum _proximityCriterion;
        private readonly double _distribParameterCorrection;

        internal SimpleAnswerCategory(Int32 weightIndex, XmlAnswerProximityCriterionEnum proximityCriterion, double distribParameterCorrection) : base(weightIndex)
        {
            _proximityCriterion = proximityCriterion;
            _distribParameterCorrection = distribParameterCorrection;
        }

        internal int QuestionCount
        {
            get { return _questionList.Count; }
        }

        internal int DistinctAnswerCount
        {
            get { return _answerDictionary.Count; }
        }

        internal int TotalAnswerCount
        {
            get { return _answerList.Count; }
        }

        internal void addAnswer(PossibleAnswer answer)
        {
            _answerList.Add(answer);
            string answerString = answer.AttributeValue.Value;
            if (!_answerDictionary.ContainsKey(answerString)) _answerDictionary.Add(answerString, new List<PossibleAnswer>());
            _answerDictionary[answerString].Add(answer);
        }

        internal void addQuestion(SimpleAnswerQuestion question)
        {
            _questionList.Add(question);
        }

        internal void setComments(XmlAttributeType attributeType)
        {
            /*std::map < std::wstring, std::vector <const PossibleAnswer*> >::iterator it = _answerMap.begin();
            for (; it != _answerMap.end(); ++it)*/
            foreach (List<PossibleAnswer> answerList in _answerDictionary.Values)
            {
                string comment = "";
                /*std::vector <const PossibleAnswer*> answerVector = (*it).second;
                int i, n = answerVector.size();
                for (i = 0; i < n; ++i)*/
                foreach (PossibleAnswer answer in answerList)
                {
                    /*const PossibleAnswer* answer = answerVector[i];
                    if (attributeType == 0)
                    {
                        comment += answer->getElement()->getName();
                        if (i != n - 1) comment += L", ";
                    }
                    else
                    {
                        const AttributeValue* attributeValue = answer->getElement()->getAttributeValue(attributeType);
                        if (attributeValue != 0)
                        {
                            comment += attributeValue->getValue();
                            if (i != n - 1) comment += L", ";
                        }
                    }*/
                }

                //for (i = 0; i < n; ++i) answerVector[i]->setComment(comment);
            }
        }
    }
}
