using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class SimpleAnswerCategory : Category
    {
        //private readonly List<Choice> _choiceList = new List<Choice>();
        private readonly SortedDictionary<Text, List<Choice>> _choiceDictionary = new SortedDictionary<Text, List<Choice>>(new TextComparer());
        private readonly List<SimpleAnswerQuestion> _questionList = new List<SimpleAnswerQuestion>();
        private readonly XmlAnswerProximityCriterionEnum _proximityCriterion;
        private readonly double _distribParameterCorrection;

        internal SimpleAnswerCategory(Int32 weightIndex, XmlAnswerProximityCriterionEnum proximityCriterion, double distribParameterCorrection, string questionNameInLog)
            : base(weightIndex, questionNameInLog)
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

        /*internal int TotalChoiceCount
        {
            get { return _choiceList.Count; }
        }*/

        internal void addChoice(Choice choice)
        {
            //_choiceList.Add(choice);

            Text choiceText = choice.AttributeValue.Value;
            if (!_choiceDictionary.ContainsKey(choiceText)) _choiceDictionary.Add(choiceText, new List<Choice>());
            _choiceDictionary[choiceText].Add(choice);
        }

        internal void addQuestion(SimpleAnswerQuestion question)
        {
            _questionList.Add(question);
        }

        internal void setComments(XmlAttributeType attributeType, QuizData quizData)
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

                Text commentText = Text.fromTextList(textList, quizData);

                foreach (Choice choice in choiceList) choice.Comment = commentText;
            }
        }

        internal void setComments(RelationType relationType, XmlAttributeType attributeType, QuizData quizData)
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
                    if (qElement != null) questionElementDictionary.Add(qElement, 0);

                    int i, n = choiceElement.getLinkedNElementCount(relationType);
                    for (i = 0; i < n; ++i) questionElementDictionary.Add(choiceElement.getLinkedNElement(relationType, i), 0);

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

                    choice.Comment = Text.fromTextList(textList, quizData);
                }
            }
        }

        internal void setComments(RelationType relationType, RelationType relation2Type, XmlAttributeType attributeType, QuizData quizData)
        {
            throw new NotImplementedException();
        }

        internal override BsonDocument getBsonDocument(IMongoDatabase database, string questionnaireId)
        {
            IMongoCollection<BsonDocument> questionListsCollection = database.GetCollection<BsonDocument>("question_lists");
            BsonDocument questionListDocument = getQuestionListDocument(questionnaireId);
            questionListsCollection.InsertOne(questionListDocument);

            IMongoCollection<BsonDocument> choiceListsCollection = database.GetCollection<BsonDocument>("choice_lists");
            BsonDocument choiceListDocument = getChoiceListDocument(questionnaireId);
            choiceListsCollection.InsertOne(choiceListDocument);

            string proximityCriterionType = "none";
            if (_proximityCriterion == XmlAnswerProximityCriterionEnum.SORT_KEY)
            {
                proximityCriterionType = "string";
            }
            else if (_proximityCriterion == XmlAnswerProximityCriterionEnum.ATTRIBUTE_VALUE_AS_NUMBER)
            {
                proximityCriterionType = "number";
            }
            else if (_proximityCriterion == XmlAnswerProximityCriterionEnum.ELEMENT_LOCATION)
            {
                proximityCriterionType = "3d_point";
            }
            else
            {
                throw new NotImplementedException();
            }

            BsonDocument categoryDocument = new BsonDocument()
            {
                { "type", "SimpleAnswer" },
                { "question_count", _questionList.Count },
                { "question_list", questionListDocument.GetValue("_id") },
                { "choice_count", _choiceDictionary.Count },
                { "choice_list", choiceListDocument.GetValue("_id") },
                { "weight_index", WeightIndex },
                { "distrib_parameter_correction", _distribParameterCorrection },
                { "proximity_criterion_type", proximityCriterionType }
            };

            return categoryDocument;
        }

        internal BsonDocument getQuestionListDocument(string questionnaireId)
        {
            BsonDocument questionListDocument = new BsonDocument();

            BsonArray questionsArray = new BsonArray();
            foreach (SimpleAnswerQuestion question in _questionList)
            {
                questionsArray.Add(question.getBsonDocument());
            }

            BsonDocument questionsDocument = new BsonDocument()
            {
                { "questionnaire", questionnaireId },
                { "count", _questionList.Count },
                { "questions", questionsArray }
            };

            questionListDocument.AddRange(questionsDocument);

            return questionListDocument;
        }

        internal BsonDocument getChoiceListDocument(string questionnaireId)
        {
            BsonArray choicesArray = new BsonArray();
            foreach (List<Choice> list in _choiceDictionary.Values)
            {
                BsonArray proximityCriterionArray = new BsonArray();
                if (_proximityCriterion == XmlAnswerProximityCriterionEnum.SORT_KEY)
                {
                    foreach (Choice choice in list) proximityCriterionArray.Add(choice.Element.XmlElement.sortKey);
                }
                else if (_proximityCriterion == XmlAnswerProximityCriterionEnum.ATTRIBUTE_VALUE_AS_NUMBER)
                {
                    foreach (Choice choice in list)
                    {
                        Double? d = choice.AttributeValue.Value.getAsDouble();
                        if (d == null)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                                "Category \"{0}\" (with answerProximityCriterion=\"ATTRIBUTE_VALUE_AS_NUMBER\"), choice {1}: Fail to convert attribute value to number, value 0 is used as proximity criterion",
                                QuestionNameInLog, choice.Element.XmlElement.id));
                            proximityCriterionArray.Add(0);
                        }
                        else
                        {
                            proximityCriterionArray.Add(d);
                        }
                    }
                }
                else
                {
                    foreach (Choice choice in list) proximityCriterionArray.Add(choice.Element.GeoPoint.getBsonDocument());
                }

                BsonDocument choiceDocument = new BsonDocument()
                {
                    { "choice", list[0].AttributeValue.Value.getBsonDocument() },
                    { "comment", list[0].Comment.getBsonDocument() },
                    { "proximity_criterion_values", proximityCriterionArray }
                };
                choicesArray.Add(choiceDocument);
            }

            BsonDocument choicesDocument = new BsonDocument()
            {
                { "questionnaire", questionnaireId },
                { "count", _choiceDictionary.Count },
                { "choices", choicesArray }
            };

            BsonDocument choiceListDocument = new BsonDocument();
            choiceListDocument.AddRange(choicesDocument);

            return choiceListDocument;
        }
    }
}
