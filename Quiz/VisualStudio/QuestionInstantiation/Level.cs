using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class Level
    {
        private XmlLevel _xmlLevel;
        private QuizData _quizData;
        private Text _name = new Text();
        private string _nameInLog;
        private Int32 _value;
        private Int32 _weightSum = 0;
        private Int32 _choiceCount;
        private Int32 _totalQuestionCount = 0;
        private readonly Dictionary<string, Element> _elementDictionary = new Dictionary<string, Element>();
        private readonly Dictionary<XmlElementType, List<Element>> _elementByTypeDictionary = new Dictionary<XmlElementType, List<Element>>();
        private readonly List<Category> _categoryList = new List<Category>();

        internal int initialize(QuizData quizData, XmlLevel xmlLevel)
        {
            _xmlLevel = xmlLevel;
            _quizData = quizData;

            _nameInLog = _xmlLevel.name[0].text;
            foreach (XmlName xmlName in _xmlLevel.name) _name.setText(xmlName.language, xmlName.text);
            quizData.verifyText(_name, String.Format("Level {0}", _nameInLog));

            _value = Int32.Parse(_xmlLevel.value);
            _choiceCount = Int32.Parse(_xmlLevel.choiceCount);

            if (addElements() != 0) return -1;
            if (checkSymetricalRelations() != 0) return -1;
            if (addQuestions() != 0) return -1;

            return 0;
        }

        internal Int32 QuestionCount { get { return _totalQuestionCount; } }

        private int addElements()
        {
            SortedDictionary<Text, List<Element>> elementByAttributeKeyDictionary = new SortedDictionary<Text, List<Element>>(new TextComparer());

            foreach (XmlElement xmlElement in _quizData.XmlQuizData.elementList)
            {
                Int32 minLevel = Int32.Parse(xmlElement.minLevel);
                if (_value >= minLevel)
                {
                    XmlElementType xmlElementType = _quizData.getXmlElementType(xmlElement.type);
                    Element element = new Element(xmlElement, xmlElementType);

                    if (element.setName(_quizData) != 0) return -1;
                    if (element.addAttributes(_quizData) != 0) return -1;
                    if (element.addNumericalAttributes(_quizData) != 0) return -1;

                    _elementDictionary.Add(xmlElement.id, element);

                    foreach (Text attributeKey in element.AttributeKeyList)
                    {
                        if (!elementByAttributeKeyDictionary.ContainsKey(attributeKey)) elementByAttributeKeyDictionary.Add(attributeKey, new List<Element>());
                        elementByAttributeKeyDictionary[attributeKey].Add(element);
                    }
                }
            }

            foreach (KeyValuePair<Text, List<Element>> pair in elementByAttributeKeyDictionary)
            {
                List<Element> elementList = pair.Value;

                if (elementList.Count > 1)
                {
                    Text key = pair.Key;
                    string eltTypeStr = null, attrTypeStr = null;
                    List<string> valueStrList = new List<string>();
                    foreach (string language in key.LanguageList)
                    {
                        string text = key.getText(language);   
                        eltTypeStr = text.Substring(0, text.IndexOf('+'));
                        attrTypeStr = text.Substring(text.IndexOf('+') + 1);
                        valueStrList.Add(attrTypeStr.Substring(attrTypeStr.IndexOf('+') + 1));
                        attrTypeStr = attrTypeStr.Substring(0, attrTypeStr.IndexOf('+'));
                    }

                    foreach (Element element in elementList)
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                            "Level \"{0}\": Element {1} has the same value in at least one language ({2}) for attribute {3} as another element of same type ({4}). Since the attribute type {3} can be used as a question, the element {1} is ignored to avoid ambiguous questions",
                            _nameInLog, element.XmlElement.id, String.Join("|", valueStrList), attrTypeStr, eltTypeStr));

                        _elementDictionary.Remove(element.XmlElement.id);
                    }
                }
            }

            foreach (KeyValuePair<string, Element> pair in _elementDictionary)
            {
                Element element = pair.Value;

                if (element.addRelations(_quizData, _elementDictionary) == 0)
                {
                    if (!_elementByTypeDictionary.ContainsKey(element.Type)) _elementByTypeDictionary.Add(element.Type, new List<Element>());
                    _elementByTypeDictionary[element.Type].Add(element);
                }
                else
                {
                    return -1;
                }
            }

            return 0;
        }

        int checkSymetricalRelations()
        {
            foreach (RelationType relationType in _quizData.RelationTypeDictionary.Values)
            {
                if (relationType.Nature == RelationNatureEnum.RELATION_NN && relationType.CheckSymetry != XmlCheckSymetryEnum.NO)
                {
                    Dictionary<Element, Dictionary<Element, int>> elementPairDictionary = new Dictionary<Element, Dictionary<Element, int>>();

                    foreach (Element element in _elementDictionary.Values)
                    {
                        int i, n = element.getLinkedNElementCount(relationType);
                        for (i = 0; i < n; ++i)
                        {
                            Element linkedElement = element.getLinkedNElement(relationType, i);

                            if (elementPairDictionary.ContainsKey(linkedElement) && elementPairDictionary[linkedElement].ContainsKey(element))
                            {
                                elementPairDictionary[linkedElement].Remove(element);
                            }
                            else
                            {
                                if (!elementPairDictionary.ContainsKey(element)) elementPairDictionary.Add(element, new Dictionary<Element, int>());
                                elementPairDictionary[element].Add(linkedElement, 0);
                            }
                        }
                    }

                    foreach (KeyValuePair<Element, Dictionary<Element, int>> pair1 in elementPairDictionary)
                    {
                        Element element1 = pair1.Key;
                        foreach (KeyValuePair<Element, int> pair2 in pair1.Value)
                        {
                            Element element2 = pair2.Key;
                            string message = String.Format("Level \"{0}\": Relation {1}: Symetry check failed for elements {2} and {3}",
                                _nameInLog, relationType.Id, element1.XmlElement.id, element2.XmlElement.id);

                            if (relationType.CheckSymetry == XmlCheckSymetryEnum.YES_ERROR)
                            {
                                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, message);
                                return -1;
                            }
                            else
                            {
                                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, message);
                            }
                        }
                    }
                }
            }

            return 0;
        }

        int addQuestions()
        {
            if (addAttributeQuestions() != 0) return -1;

            /*if (!addAttributeQuestions()) return false;
            if (!addAttributeOrderQuestions()) return false;
            if (!addRelation1Questions()) return false;
            if (!addRelationNQuestions()) return false;
            if (!addRelationLimitQuestions()) return false;
            if (!addRelationOrderQuestions()) return false;
            if (!addRelationExistenceQuestions()) return false;

            const wchar_t *cMessage;
            std::wstring message;
            wchar_t countStr[16];

            std::map<const ElementType *, std::vector<const Element *> >::iterator it = _elementByTypeMap.begin();
            for (; it!=_elementByTypeMap.end(); ++it)
            {
                const std::wstring typeName = (*it).first->getId();
                unsigned int elementCount = (*it).second.size();
                cMessage = XmlFunctions::get_informationMessages_elementCountInfo();
                message = cMessage;
                swprintf (countStr, 16, L"%d", elementCount);
                message.replace (message.find ('%'), 1, typeName);
                message.replace (message.find ('#'), 1, countStr);
                MessageLogger::instance()->addMessage (MESSAGE, message.c_str());
            }

            cMessage = XmlFunctions::get_informationMessages_totalQuestionCountInfo();
            message = cMessage;
            swprintf (countStr, 16, L"%d", _totalQuestionCount);
            message.replace (message.find ('%'), 1, _name);
            message.replace (message.find ('#'), 1, countStr);
            MessageLogger::instance()->addMessage (MESSAGE, message.c_str());

            cMessage = XmlFunctions::get_informationMessages_categoryCountInfo();
            message = cMessage;
            swprintf (countStr, 16, L"%d", _categoryVector.size());
            message.replace (message.find ('%'), 1, _name);
            message.replace (message.find ('#'), 1, countStr);
            MessageLogger::instance()->addMessage (MESSAGE, message.c_str());*/

            return 0;
        }

        int addAttributeQuestions()
        {
            foreach(XmlAttributeQuestionCategory xmlAttributeQuestionCategory in _quizData.XmlQuizData.questionCategories.attributeQuestionCategoryList)
            {
                Int32 minLevel = Int32.Parse(xmlAttributeQuestionCategory.minLevel);
                if (_value >= minLevel)
                {
                    XmlElementType elementType = _quizData.getXmlElementType(xmlAttributeQuestionCategory.elementType);

                    string questionNameInLog =  xmlAttributeQuestionCategory.questionText[0].text;

                    if (_elementByTypeDictionary.ContainsKey(elementType))
                    {
                        XmlAttributeType questionAttributeType = _quizData.getXmlAttributeType(xmlAttributeQuestionCategory.questionAttribute);
                        if (!questionAttributeType.canBeQuestion)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Error in {0}: Error in definition of category \"{1}\": Attribute {2} can not be used as a question",
                                _quizData.DataFileName, questionNameInLog, questionAttributeType.id));
                            return -1;
                        }

                        XmlAttributeType answerAttributeType = _quizData.getXmlAttributeType(xmlAttributeQuestionCategory.answerAttribute);
                        Int32 weight = Int32.Parse(xmlAttributeQuestionCategory.weight);
                        _weightSum += weight;

                        double distribParameterCorrection = 0.0;
                        if (xmlAttributeQuestionCategory.distribParameterCorrectionSpecified) distribParameterCorrection = xmlAttributeQuestionCategory.distribParameterCorrection;
                        SimpleAnswerCategory category = new SimpleAnswerCategory(_weightSum, xmlAttributeQuestionCategory.answerProximityCriterion, distribParameterCorrection);

                        foreach (Element element in _elementByTypeDictionary[elementType])
                        {
                            AttributeValue questionAttributeValue = element.getAttributeValue(questionAttributeType);
                            AttributeValue answerAttributeValue = element.getAttributeValue(answerAttributeType);

                            if (answerAttributeValue != null) 
                            {
                                PossibleAnswer answer = new PossibleAnswer(answerAttributeValue, element);
                                category.addAnswer(answer);

                                if (questionAttributeValue != null)
                                {
                                    Text questionText = new Text();
                                    foreach (XmlQuestionText xmlQuestionText in xmlAttributeQuestionCategory.questionText)
                                    {
                                        string languageId = xmlQuestionText.language;
                                        string questionString = String.Format(xmlQuestionText.text, questionAttributeValue.Value.getText(languageId));
                                        questionText.setText(languageId, questionString);
                                    }
                                    if (_quizData.verifyText(questionText, String.Format("question {0}", questionNameInLog)) != 0) return -1;

                                    SimpleAnswerQuestion question = new SimpleAnswerQuestion(questionText, answer, null);
                                    category.addQuestion(question);
                                }
                            }
                        }

                        if (category.QuestionCount == 0)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Level \"{0}\": No question in category \"{1}\". The category is ignored",
                                _nameInLog, questionNameInLog));
                            _weightSum -= weight;
                        }
                        else if (category.DistinctAnswerCount < _choiceCount)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                                "Level \"{0}\", category \"{1}\": Not enough possible answers ({2} possibles answers, {3} required). The category is ignored",
                                _nameInLog, questionNameInLog, category.DistinctAnswerCount, _choiceCount));
                            _weightSum -= weight;
                        }
                        else
                        {
                            if (xmlAttributeQuestionCategory.commentMode == XmlCommentModeEnum.QUESTION_ATTRIBUTE)
                            {
                                if (category.setComments(questionAttributeType, _quizData) != 0) return -1;
                            }
                            else if (xmlAttributeQuestionCategory.commentMode == XmlCommentModeEnum.NAME)
                            {
                                if (category.setComments(null, _quizData) != 0) return -1;
                            }

                            MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Level \"{0}\", category \"{1}\": {2} question(s), {3} possible answer(s)",
                                _nameInLog, questionNameInLog, category.QuestionCount, category.DistinctAnswerCount));

                            _categoryList.Add(category);
                            _totalQuestionCount += category.QuestionCount;
                        }
                    }
                    else
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                                "Level \"{0}\": No question in category \"{1}\" because there is no element of type {2}. The category is ignored",
                                _nameInLog, questionNameInLog, elementType.id));
                    }
                }
            }

            return 0;
        }

        internal int fillDataBase(IMongoDatabase database)
        {
            IMongoCollection<BsonDocument> levelCollection = database.GetCollection<BsonDocument>("levels");

            BsonDocument levelDocument = new BsonDocument()
            {
                { "questionnaire", _quizData.XmlQuizData.parameters.questionnaireId },
                { "levelId", _xmlLevel.levelId },
                { "index", _xmlLevel.value },
                { "name", _name.getBsonDocument() }
            };

            BsonArray categoriesArray = new BsonArray();
            foreach (Category category in _categoryList)
            {
                categoriesArray.Add(category.getBsonDocument(database, _quizData.XmlQuizData.parameters.questionnaireId));
            }

            BsonDocument categoriesDocument = new BsonDocument()
            {
                { "categories", categoriesArray }
            };

            levelDocument.AddRange(categoriesDocument);
            levelCollection.InsertOne(levelDocument);

            return 0;
        }
    }
}
