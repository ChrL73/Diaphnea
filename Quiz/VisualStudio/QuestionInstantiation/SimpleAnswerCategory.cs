using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace QuestionInstantiation
{
    class SimpleAnswerCategory : Category
    {
        private readonly SortedDictionary<Text, List<Choice>> _choiceDictionary = new SortedDictionary<Text, List<Choice>>(new TextComparer());
        private readonly List<SimpleAnswerQuestion> _questionList = new List<SimpleAnswerQuestion>();
        private readonly XmlSimpleAnswerProximityCriterionEnum _proximityCriterion;
        private readonly double _distribParameterCorrection;

        internal SimpleAnswerCategory(Int32 weightIndex, string questionNameInLog, QuizData quizData, XmlSimpleAnswerProximityCriterionEnum proximityCriterion,
                                      double distribParameterCorrection, MapParameters mapParameters)
            : base(weightIndex, questionNameInLog, quizData, mapParameters)
        {
            _proximityCriterion = proximityCriterion;
            _distribParameterCorrection = distribParameterCorrection;
        }

        internal int QuestionCount
        {
            get { return _questionList.Count; }
        }

        internal int ChoiceCount
        {
            get { return _choiceDictionary.Count; }
        }

        internal void addChoice(Choice choice)
        {
            Text choiceText = choice.AttributeValue.Value;
            if (!_choiceDictionary.ContainsKey(choiceText)) _choiceDictionary.Add(choiceText, new List<Choice>());
            _choiceDictionary[choiceText].Add(choice);
        }

        internal void addQuestion(SimpleAnswerQuestion question)
        {
            _questionList.Add(question);
        }

        internal void setComments(XmlAttributeType attributeType)
        {
            foreach (List<Choice> choiceList in _choiceDictionary.Values)
            {
                List<Text> textList = new List<Text>();
                foreach (Choice choice in choiceList)
                {
                    if (attributeType == null)
                    {
                        textList.Add(choice.Element.Name);
                    }
                    else
                    {
                        AttributeValue attributeValue = choice.Element.getAttributeValue(attributeType);
                        if (attributeValue != null) textList.Add(attributeValue.Value);
                    }
                }

                Text commentText = Text.fromTextList(textList, QuizData);

                foreach (Choice choice in choiceList) choice.Comment = commentText;
            }
        }

        internal void setComments(RelationType relationType, XmlAttributeType attributeType)
        {
            relationType = relationType.ReciprocalType;

            foreach (List<Choice> choiceList in _choiceDictionary.Values)
            {
                if (choiceList.Count == 1)
                {
                    List<Text> textList = new List<Text>();
                    Choice choice = choiceList[0];
                    Element choiceElement = choice.Element;
                    Dictionary<Element, int> questionElementDictionary = new Dictionary<Element, int>();

                    Element qElement = choiceElement.getLinked1Element(relationType);
                    if (qElement != null) questionElementDictionary[qElement] = 0;

                    int i, n = choiceElement.getLinkedNElementCount(relationType);
                    for (i = 0; i < n; ++i) questionElementDictionary[choiceElement.getLinkedNElement(relationType, i)] = 0;

                    foreach (Element questionElement in questionElementDictionary.Keys)
                    {    
                        if (attributeType == null)
                        {
                            textList.Add(questionElement.Name);
                        }
                        else
                        {
                            AttributeValue attributeValue = questionElement.getAttributeValue(attributeType);
                            if (attributeValue != null) textList.Add(attributeValue.Value);
                        }
                    }

                    choice.Comment = Text.fromTextList(textList, QuizData);
                }
            }
        }

        internal void setComments(RelationType relationType, RelationType relation2Type, XmlAttributeType attributeType)
        {
            throw new NotImplementedException();
        }

        internal override int generateCode(List<CodeGenerator> codeGeneratorList)
        {
            foreach (CodeGenerator codeGenerator in codeGeneratorList)
            {
                List<int> choiceList = new List<int>();
                List<int> questionList = new List<int>();

                foreach (List<Choice> list in _choiceDictionary.Values)
                {
                    choiceList.Add(codeGenerator.addSimpleAnswerChoice(list, _proximityCriterion, QuestionNameInLog));
                }

                foreach (SimpleAnswerQuestion question in _questionList)
                {
                    questionList.Add(codeGenerator.addSimpleAnswerQuestion(question, _proximityCriterion));
                }

                int mapParamOffset = codeGenerator.addMapParameters(MapParameters);

                codeGenerator.addSimpleAnswerCategory(WeightIndex, mapParamOffset, questionList, choiceList, _distribParameterCorrection, _proximityCriterion);
            }

            return 0;
        }
    }
}
