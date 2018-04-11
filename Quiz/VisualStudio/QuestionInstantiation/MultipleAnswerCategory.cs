using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace QuestionInstantiation
{
    class MultipleAnswerCategory : Category
    {
        private readonly SortedDictionary<Text, List<Choice>> _choiceDictionary = new SortedDictionary<Text, List<Choice>>(new TextComparer());
        private readonly List<MultipleAnswerQuestion> _questionList = new List<MultipleAnswerQuestion>();
        private readonly XmlMultipleAnswerProximityCriterionEnum _proximityCriterion;
        private readonly double _distribParameterCorrection;

        internal MultipleAnswerCategory(Int32 weightIndex, string questionNameInLog, QuizData quizData, XmlMultipleAnswerProximityCriterionEnum proximityCriterion,
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

        internal void addQuestion(MultipleAnswerQuestion question)
        {
            _questionList.Add(question);
        }

        internal void setComments(RelationType relationType, XmlAttributeType attributeType)
        {
            // Following code is the same as in SimpleAnswerQuestion.setComments(RelationType relationType, XmlAttributeType attributeType)
            // => Todo: This code should not be duplicated
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
            RelationType tempRelationType = relationType;
            relationType = relation2Type.ReciprocalType;
            relation2Type = tempRelationType.ReciprocalType;

            foreach (List<Choice> choiceList in _choiceDictionary.Values)
            {
                if (choiceList.Count == 1)
                {
                    List<Text> textList = new List<Text>();
                    Choice choice = choiceList[0];
                    Element choiceElement = choice.Element;
                    Dictionary<Element, int> middleElementDictionary = new Dictionary<Element, int>();
                    Dictionary<Element, int> questionElementDictionary = new Dictionary<Element, int>();

                    Element mElement = choiceElement.getLinked1Element(relationType);
                    if (mElement != null) middleElementDictionary[mElement] = 0;

                    int i, n = choiceElement.getLinkedNElementCount(relationType);
                    for (i = 0; i < n; ++i) middleElementDictionary[choiceElement.getLinkedNElement(relationType, i)] = 0;

                    foreach(Element middleElement in middleElementDictionary.Keys)
                    {
                        Element qElement = middleElement.getLinked1Element(relation2Type);
                        if (qElement != null) questionElementDictionary[qElement] = 0;

                        int j, m = middleElement.getLinkedNElementCount(relation2Type);
                        for (j = 0; j < m; ++j) questionElementDictionary[middleElement.getLinkedNElement(relation2Type, j)] = 0;
                    }

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

        internal override BsonDocument getBsonDocument(IMongoDatabase database, string questionnaireId)
        {
            IMongoCollection<BsonDocument> questionListsCollection = database.GetCollection<BsonDocument>("question_lists");
            BsonDocument questionListDocument = getQuestionListDocument(questionnaireId);
            questionListsCollection.InsertOne(questionListDocument);

            IMongoCollection<BsonDocument> choiceListsCollection = database.GetCollection<BsonDocument>("choice_lists");
            BsonDocument choiceListDocument = getChoiceListDocument(questionnaireId);
            choiceListsCollection.InsertOne(choiceListDocument);

            string proximityCriterionType = "none";
            if (_proximityCriterion == XmlMultipleAnswerProximityCriterionEnum.ELEMENT_LOCATION)
            {
                proximityCriterionType = "3d_point";
            }

            BsonDocument categoryDocument = new BsonDocument()
            {
                { "type", "MultipleAnswer" },
                { "question_count", _questionList.Count },
                { "question_list", questionListDocument.GetValue("_id") },
                { "choice_count", _choiceDictionary.Count },
                { "choice_list", choiceListDocument.GetValue("_id") },
                { "weight_index", WeightIndex },
                { "distrib_parameter_correction", _distribParameterCorrection },
                { "proximity_criterion_type", proximityCriterionType },
                { "map_parameters", getMapParameterBsonDocument() }
            };

            return categoryDocument;
        }

        internal BsonDocument getQuestionListDocument(string questionnaireId)
        {
            BsonDocument questionListDocument = new BsonDocument();

            BsonArray questionsArray = new BsonArray();
            foreach (MultipleAnswerQuestion question in _questionList)
            {
                questionsArray.Add(question.getBsonDocument(QuizData));
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
                BsonDocument choiceDocument = new BsonDocument()
                {
                    { "choice", list[0].AttributeValue.Value.getBsonDocument() },
                    { "comment", list[0].Comment.getBsonDocument() },
                    { "map_id", list[0].Element.XmlElement.mapId == null ? "" : list[0].Element.XmlElement.mapId.Substring(2) }
                };

                if (_proximityCriterion != XmlMultipleAnswerProximityCriterionEnum.NONE)
                {
                    BsonArray proximityCriterionArray = new BsonArray();
                    if (_proximityCriterion == XmlMultipleAnswerProximityCriterionEnum.ELEMENT_LOCATION)
                    {
                        foreach (Choice choice in list) proximityCriterionArray.Add(choice.Element.GeoPoint.getBsonDocument());
                    }

                    choiceDocument.AddRange(new BsonDocument() { { "proximity_criterion_values", proximityCriterionArray } });
                }

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

        internal override int generateCode(List<CodeGenerator> codeGeneratorList)
        {
            foreach (CodeGenerator codeGenerator in codeGeneratorList)
            {
                List<int> choiceList = new List<int>();
                List<int> questionList = new List<int>();

                foreach (List<Choice> list in _choiceDictionary.Values)
                {
                    choiceList.Add(codeGenerator.addMultipleAnswerChoice(list, _proximityCriterion));
                }

                foreach (MultipleAnswerQuestion question in _questionList)
                {
                    questionList.Add(codeGenerator.addMultipleAnswerQuestion(question, _proximityCriterion));
                }

                int mapParamOffset = codeGenerator.addMapParameters(MapParameters);

                codeGenerator.addMultipleAnswerCategory(WeightIndex, mapParamOffset, questionList, choiceList, _distribParameterCorrection, _proximityCriterion);
            }

            return 0;
        }
    }
}
