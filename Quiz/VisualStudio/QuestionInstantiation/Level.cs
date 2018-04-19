using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Schemas;

namespace QuestionInstantiation
{
    class Level
    {
        private XmlLevel _xmlLevel;
        private QuizData _quizData;
        private bool _testMode;
        private Text _name = new Text();
        private string _nameInLog;
        private int _logLanguageIndex = 0;
        private Int32 _value;
        private Int32 _weightSum = 0;
        private Int32 _choiceCount;
        private Int32 _questionCount;
        private Int32 _totalQuestionCount = 0;
        private readonly Dictionary<string, Element> _elementDictionary = new Dictionary<string, Element>();
        private readonly Dictionary<XmlElementType, List<Element>> _elementByTypeDictionary = new Dictionary<XmlElementType, List<Element>>();
        private readonly List<Category> _categoryList = new List<Category>();

        internal int initialize(QuizData quizData, XmlLevel xmlLevel, bool testMode)
        {
            _xmlLevel = xmlLevel;
            _quizData = quizData;
            _testMode = testMode;

            int i = 0;
            foreach (XmlLanguage language in quizData.XmlQuizData.parameters.languageList)
            {
                if (language.id == XmlLanguageId.en)
                {
                    _logLanguageIndex = i;
                    break;
                }

                ++i;
            }

            _nameInLog = _xmlLevel.name[_logLanguageIndex].text;
            foreach (XmlName xmlName in _xmlLevel.name) _name.setText(xmlName.language.ToString(), xmlName.text);
            if (quizData.verifyText(_name, String.Format("Level {0}", _nameInLog)) != 0) return -1;

            _value = Int32.Parse(_xmlLevel.value);
            _choiceCount = Int32.Parse(_xmlLevel.choiceCount);
            _questionCount = Int32.Parse(_xmlLevel.questionCount);

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

        private int checkSymetricalRelations()
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

        private int addQuestions()
        {
            if (addAttributeQuestions() != 0) return -1;
            if (addRelation1Questions() != 0) return -1;
            if (addRelationNQuestions() != 0) return -1;
            if (addAttributeOrderQuestions() != 0) return -1;
            if (addRelationLimitQuestions() != 0) return -1;
            if (addRelationOrderQuestions() != 0) return -1;
            if (addRelationExistenceQuestions() != 0) return -1;

            foreach(KeyValuePair<XmlElementType, List<Element>> pair in _elementByTypeDictionary)
            {
                string typeName = pair.Key.id;
                int elementCount = pair.Value.Count;
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Number of element(s) for type \"{0}\": {1}", typeName, elementCount));
            }

            MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Total number of question(s) for level \"{0}\": {1}", _nameInLog, _totalQuestionCount));

            int categoryCount = _categoryList.Count;
            MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Total number of categor{0} for level \"{1}\": {2}", categoryCount > 1 ? "ies" : "y", _nameInLog, categoryCount));

            return 0;
         }

        private int addAttributeQuestions()
        {
            foreach (XmlAttributeQuestionCategory xmlAttributeQuestionCategory in _quizData.XmlQuizData.questionCategories.attributeQuestionCategoryList)
            {
                Int32 minLevel = Int32.Parse(xmlAttributeQuestionCategory.minLevel);
                if (_value >= minLevel && (!_testMode || xmlAttributeQuestionCategory.test))
                {
                    XmlElementType elementType = _quizData.getXmlElementType(xmlAttributeQuestionCategory.elementType);

                    string questionNameInLog =  xmlAttributeQuestionCategory.questionText[_logLanguageIndex].text;

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
                        MapParameters mapParameters = new MapParameters(xmlAttributeQuestionCategory.mapParameters);
                        SimpleAnswerCategory category = new SimpleAnswerCategory(_weightSum, questionNameInLog, _quizData, xmlAttributeQuestionCategory.answerProximityCriterion,
                                                                                 distribParameterCorrection, mapParameters);

                        foreach (Element element in _elementByTypeDictionary[elementType])
                        {
                            AttributeValue questionAttributeValue = element.getAttributeValue(questionAttributeType);
                            AttributeValue answerAttributeValue = element.getAttributeValue(answerAttributeType);

                            if (answerAttributeValue != null) 
                            {
                                if (xmlAttributeQuestionCategory.answerProximityCriterion != XmlSimpleAnswerProximityCriterionEnum.ELEMENT_LOCATION || element.GeoPoint != null)
                                {
                                    Choice choice = new Choice(answerAttributeValue, element, _quizData);
                                    category.addChoice(choice);

                                    if (questionAttributeValue != null)
                                    {
                                        Text questionText = new Text();
                                        foreach (XmlQuestionText1P xmlQuestionText in xmlAttributeQuestionCategory.questionText)
                                        {
                                            string languageId = xmlQuestionText.language.ToString();
                                            string questionString = String.Format(xmlQuestionText.text, questionAttributeValue.Value.getText(languageId));
                                            questionText.setText(languageId, questionString);
                                        }
                                        if (_quizData.verifyText(questionText, String.Format("question {0}", questionNameInLog)) != 0) return -1;

                                        SimpleAnswerQuestion question = new SimpleAnswerQuestion(questionText, choice, null, null, xmlAttributeQuestionCategory.answerProximityCriterion);
                                        category.addQuestion(question);
                                    }
                                }
                            }
                        }

                        if (category.QuestionCount == 0)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Level \"{0}\": No question in category \"{1}\". The category is ignored",
                                _nameInLog, questionNameInLog));
                            _weightSum -= weight;
                        }
                        else if (category.ChoiceCount < _choiceCount)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                                "Level \"{0}\", category \"{1}\": Not enough choices ({2} choices, {3} required). The category is ignored",
                                _nameInLog, questionNameInLog, category.ChoiceCount, _choiceCount));
                            _weightSum -= weight;
                        }
                        else
                        {
                            if (xmlAttributeQuestionCategory.commentMode == XmlCommentModeEnum.QUESTION_ATTRIBUTE)
                            {
                                category.setComments(questionAttributeType);
                            }
                            else if (xmlAttributeQuestionCategory.commentMode == XmlCommentModeEnum.NAME)
                            {
                                category.setComments(null);
                            }

                            MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Level \"{0}\", category \"{1}\": {2} question(s), {3} choice(s)",
                                _nameInLog, questionNameInLog, category.QuestionCount, category.ChoiceCount));

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

        private int addRelation1Questions()
        {
            foreach (XmlRelation1QuestionCategory xmlRelation1QuestionCategory in _quizData.XmlQuizData.questionCategories.relation1QuestionCategoryList)
            {
                int minLevel = Int32.Parse(xmlRelation1QuestionCategory.minLevel);
                if (_value >= minLevel && (!_testMode || xmlRelation1QuestionCategory.test))
                {
                    RelationType relationType = _quizData.getRelationType(xmlRelation1QuestionCategory.relation);
                    if (xmlRelation1QuestionCategory.way == XmlWayEnum.INVERSE) relationType = relationType.ReciprocalType;

                    string questionNameInLog = xmlRelation1QuestionCategory.questionText[_logLanguageIndex].text;

                    if (relationType.Nature == RelationNatureEnum.RELATION_NN || (relationType.Nature == RelationNatureEnum.RELATION_1N && relationType.Way != RelationWayEnum.INVERSE))
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Error in {0}: Error in definition of category \"{1}\": Relation \"{2}\" can not define questions with one answer",
                                _quizData.DataFileName, questionNameInLog, relationType.FullName));
                        return -1;
                    }

                    if (xmlRelation1QuestionCategory.relation2 != null && !xmlRelation1QuestionCategory.way2Specified)
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format(
                                "Error in {0}: Error in definition of category \"{1}\": way2 attribute must be specified if relation2 attribute is specified",
                                _quizData.DataFileName, questionNameInLog));
                        return -1;
                    }

                    RelationType relation2Type = null;
                    if (xmlRelation1QuestionCategory.relation2 != null)
                    {
                        relation2Type = _quizData.getRelationType(xmlRelation1QuestionCategory.relation2);
                        if (xmlRelation1QuestionCategory.way2 == XmlWayEnum.INVERSE) relation2Type = relation2Type.ReciprocalType;

                        if (relation2Type.Nature == RelationNatureEnum.RELATION_NN || (relation2Type.Nature == RelationNatureEnum.RELATION_1N && relation2Type.Way != RelationWayEnum.INVERSE))
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Error in {0}: Error in definition of category \"{1}\": Relation \"{2}\" can not define questions with one answer",
                                    _quizData.DataFileName, questionNameInLog, relation2Type.FullName));
                            return -1;
                        }

                        if (relationType.EndType != relation2Type.StartType)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format(
                                "Error in {0}: Error in definition of category \"{1}\": End type of relation 1 ({2}) is different from start type of relation 2 ({3})",
                                _quizData.DataFileName, questionNameInLog, relationType.FullName, relation2Type.FullName));
                            return -1;
                        }
                    }

                    XmlElementType startElementType = relationType.StartType;
                    XmlElementType endElementType;
                    if (relation2Type != null) endElementType = relation2Type.EndType;
                    else endElementType = relationType.EndType;

                    if (_elementByTypeDictionary.ContainsKey(startElementType) && _elementByTypeDictionary.ContainsKey(endElementType))
                    {
                        XmlAttributeType questionAttributeType = _quizData.getXmlAttributeType(xmlRelation1QuestionCategory.questionAttribute);
                        if (!questionAttributeType.canBeQuestion)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Error in {0}: Error in definition of category \"{1}\": Attribute {2} can not be used as a question",
                                _quizData.DataFileName, questionNameInLog, questionAttributeType.id));
                            return -1;
                        }

                        XmlAttributeType answerAttributeType = _quizData.getXmlAttributeType(xmlRelation1QuestionCategory.answerAttribute);

                        double distribParameterCorrection = 0.0;
                        if (xmlRelation1QuestionCategory.distribParameterCorrectionSpecified) distribParameterCorrection = xmlRelation1QuestionCategory.distribParameterCorrection;

                        Int32 weight = Int32.Parse(xmlRelation1QuestionCategory.weight);
                        _weightSum += weight;

                        MapParameters mapParameters = new MapParameters(xmlRelation1QuestionCategory.mapParameters);
                        SimpleAnswerCategory category = new SimpleAnswerCategory(_weightSum, questionNameInLog, _quizData, xmlRelation1QuestionCategory.answerProximityCriterion,
                                                                                 distribParameterCorrection, mapParameters);
                        Dictionary<Element, Choice> choiceDictionary = new Dictionary<Element, Choice>();

                        foreach (Element endElement in _elementByTypeDictionary[endElementType])
                        {
                            AttributeValue answerAttributeValue = endElement.getAttributeValue(answerAttributeType);
                            if (answerAttributeValue != null)
                            {
                                if (xmlRelation1QuestionCategory.answerProximityCriterion != XmlSimpleAnswerProximityCriterionEnum.ELEMENT_LOCATION || endElement.GeoPoint != null)
                                {
                                    Choice choice = new Choice(answerAttributeValue, endElement, _quizData);
                                    category.addChoice(choice);
                                    choiceDictionary.Add(endElement, choice);
                                }
                            }
                        }

                        foreach (Element startElement in _elementByTypeDictionary[startElementType])
                        {
                            AttributeValue questionAttributeValue = startElement.getAttributeValue(questionAttributeType);
                            Element endElement = startElement.getLinked1Element(relationType);

                            if (questionAttributeValue != null && endElement != null)
                            {
                                if (relation2Type != null) endElement = endElement.getLinked1Element(relation2Type);
                                if (endElement != null)
                                {
                                    if (choiceDictionary.ContainsKey(endElement))
                                    {
                                        Choice choice = choiceDictionary[endElement];

                                        Text questionText = new Text();
                                        foreach (XmlQuestionText1P xmlQuestionText in xmlRelation1QuestionCategory.questionText)
                                        {
                                            string languageId = xmlQuestionText.language.ToString();
                                            string questionString = String.Format(xmlQuestionText.text, questionAttributeValue.Value.getText(languageId));
                                            questionText.setText(languageId, questionString);
                                        }
                                        if (_quizData.verifyText(questionText, String.Format("question {0}", questionNameInLog)) != 0) return -1;

                                        Choice ExcludedChoice = null;
                                        if (startElementType == endElementType && choiceDictionary.ContainsKey(startElement)) ExcludedChoice = choiceDictionary[startElement];

                                        SimpleAnswerQuestion question = new SimpleAnswerQuestion(questionText, choice, ExcludedChoice, startElement, xmlRelation1QuestionCategory.answerProximityCriterion);
                                        category.addQuestion(question);
                                    }
                                }
                            }
                        }

                        int choiceCount = category.ChoiceCount;
                        if (startElementType == endElementType) --choiceCount;

                        if (category.QuestionCount == 0)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Level \"{0}\": No question in category \"{1}\". The category is ignored",
                                _nameInLog, questionNameInLog));
                            _weightSum -= weight;
                        }
                        else if (choiceCount < _choiceCount)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                               "Level \"{0}\", category \"{1}\": Not enough choices ({2} choices, {3} required). The category is ignored",
                               _nameInLog, questionNameInLog, choiceCount, _choiceCount));
                            _weightSum -= weight;
                        }
                        else
                        {
                            if (xmlRelation1QuestionCategory.commentMode == XmlCommentModeEnum.QUESTION_ATTRIBUTE)
                            {
                                if (relation2Type == null) category.setComments(relationType, questionAttributeType);
                                else category.setComments(relationType, relation2Type, questionAttributeType);
                            }
                            else if (xmlRelation1QuestionCategory.commentMode == XmlCommentModeEnum.NAME)
                            {
                                if (relation2Type == null) category.setComments(relationType, null);
                                else category.setComments(relationType, relation2Type, null);
                            }

                            MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Level \"{0}\", category \"{1}\": {2} question(s), {3} choice(s)",
                                _nameInLog, questionNameInLog, category.QuestionCount, choiceCount));
                            _categoryList.Add(category);
                            _totalQuestionCount += category.QuestionCount;
                        }
                    }
                    else if (!_elementByTypeDictionary.ContainsKey(startElementType))
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                            "Level \"{0}\": No question in category \"{1}\" because there is no element of type {2}. The category is ignored",
                            _nameInLog, questionNameInLog, startElementType.id));
                    }
                    else
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                            "Level \"{0}\": No question in category \"{1}\" because there is no element of type {2}. The category is ignored",
                            _nameInLog, questionNameInLog, endElementType.id));
                    }
                }
            }

            return 0;
        }

        private int addRelationNQuestions()
        {
            foreach (XmlRelationNQuestionCategory xmlRelationNQuestionCategory in _quizData.XmlQuizData.questionCategories.relationNQuestionCategoryList)
            {
                int minLevel = Int32.Parse(xmlRelationNQuestionCategory.minLevel);
                if (_value >= minLevel && (!_testMode || xmlRelationNQuestionCategory.test))
                {
                    RelationType relationType = _quizData.getRelationType(xmlRelationNQuestionCategory.relation);
                    if (xmlRelationNQuestionCategory.way == XmlWayEnum.INVERSE) relationType = relationType.ReciprocalType;

                    string questionNameInLog = xmlRelationNQuestionCategory.questionText[_logLanguageIndex].text;

                    if (xmlRelationNQuestionCategory.relation2 != null && !xmlRelationNQuestionCategory.way2Specified)
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format(
                                "Error in {0}: Error in definition of category \"{1}\": way2 attribute must be specified if relation2 attribute is specified",
                                _quizData.DataFileName, questionNameInLog));
                        return -1;
                    }

                    RelationType relation2Type = null;
                    if (xmlRelationNQuestionCategory.relation2 != null)
                    {
                        relation2Type = _quizData.getRelationType(xmlRelationNQuestionCategory.relation2);
                        if (xmlRelationNQuestionCategory.way2 == XmlWayEnum.INVERSE) relation2Type = relation2Type.ReciprocalType;

                        if (relationType.EndType != relation2Type.StartType)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format(
                                "Error in {0}: Error in definition of category \"{1}\": End type of relation 1 ({2}) is different from start type of relation 2 ({3})",
                                _quizData.DataFileName, questionNameInLog, relationType.FullName, relation2Type.FullName));
                            return -1;
                        }

                        if ((relationType.Nature == RelationNatureEnum.RELATION_11 || (relationType.Nature == RelationNatureEnum.RELATION_1N && relationType.Way != RelationWayEnum.DIRECT)) &&
                            (relation2Type.Nature == RelationNatureEnum.RELATION_11 || (relation2Type.Nature == RelationNatureEnum.RELATION_1N && relation2Type.Way != RelationWayEnum.DIRECT)))
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format(
                                "Error in {0}: Error in definition of category \"{1}\": Relations \"{2}\" and \"{3}\" can not define questions with several answers",
                                _quizData.DataFileName, questionNameInLog, relationType.FullName, relation2Type.FullName));
                            return -1;
                        }
                    }
                    else
                    {
                        if (relationType.Nature == RelationNatureEnum.RELATION_11 || (relationType.Nature == RelationNatureEnum.RELATION_1N && relationType.Way != RelationWayEnum.DIRECT))
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format(
                                "Error in {0}: Error in definition of category \"{1}\": Relation \"{2}\" can not define questions with several answers",
                                _quizData.DataFileName, questionNameInLog, relationType.FullName));
                            return -1;
                        }
                    }

                    XmlElementType startElementType = relationType.StartType;
                    XmlElementType endElementType;
                    if (relation2Type != null) endElementType = relation2Type.EndType;
                    else endElementType = relationType.EndType;

                    if (_elementByTypeDictionary.ContainsKey(startElementType) && _elementByTypeDictionary.ContainsKey(endElementType))
                    {
                        XmlAttributeType questionAttributeType = _quizData.getXmlAttributeType(xmlRelationNQuestionCategory.questionAttribute);
                        if (!questionAttributeType.canBeQuestion)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Error in {0}: Error in definition of category \"{1}\": Attribute {2} can not be used as a question",
                                _quizData.DataFileName, questionNameInLog, questionAttributeType.id));
                            return -1;
                        }

                        XmlAttributeType answerAttributeType = _quizData.getXmlAttributeType(xmlRelationNQuestionCategory.answerAttribute);

                        double distribParameterCorrection = 0.0;
                        if (xmlRelationNQuestionCategory.distribParameterCorrectionSpecified) distribParameterCorrection = xmlRelationNQuestionCategory.distribParameterCorrection;

                        Int32 weight = Int32.Parse(xmlRelationNQuestionCategory.weight);
                        _weightSum += weight;

                        MapParameters mapParameters = new MapParameters(xmlRelationNQuestionCategory.mapParameters);
                        MultipleAnswerCategory category = new MultipleAnswerCategory(_weightSum, questionNameInLog, _quizData, xmlRelationNQuestionCategory.answerProximityCriterion,
                                                                                     distribParameterCorrection, mapParameters);
                        Dictionary<Element, Choice> choiceDictionary = new Dictionary<Element, Choice>();

                        foreach (Element endElement in _elementByTypeDictionary[endElementType])
                        {
                            AttributeValue answerAttributeValue = endElement.getAttributeValue(answerAttributeType);
                            if (answerAttributeValue != null)
                            {
                                if (xmlRelationNQuestionCategory.answerProximityCriterion != XmlMultipleAnswerProximityCriterionEnum.ELEMENT_LOCATION || endElement.GeoPoint != null)
                                {
                                    Choice choice = new Choice(answerAttributeValue, endElement, _quizData);
                                    category.addChoice(choice);
                                    choiceDictionary.Add(endElement, choice);
                                }
                            }
                        }

                        foreach (Element startElement in _elementByTypeDictionary[startElementType])
                        {
                            AttributeValue questionAttributeValue = startElement.getAttributeValue(questionAttributeType);

                            if (questionAttributeValue != null)
                            {
                                MultipleAnswerQuestion question = null;
                                Dictionary<Element, int> endElementDictionary = new Dictionary<Element, int>();
                                int k, linkedElementCount1 = startElement.getLinkedNElementCount(relationType);
                                bool relation1Is1 = false;
                                if (linkedElementCount1 == 0 && startElement.getLinked1Element(relationType) != null)
                                {
                                    relation1Is1 = true;
                                    linkedElementCount1 = 1;
                                }

                                for (k = 0; k < linkedElementCount1; ++k)
                                {
                                    Element endElement1;
                                    if (relation1Is1) endElement1 = startElement.getLinked1Element(relationType);
                                    else endElement1 = startElement.getLinkedNElement(relationType, k);
                            
                                    if (relation2Type != null)
                                    {
                                        int l, linkedElementCount2 = endElement1.getLinkedNElementCount(relation2Type);
                                        if (linkedElementCount2 == 0 && endElement1.getLinked1Element(relation2Type) != null)
                                        {
                                            endElementDictionary[endElement1.getLinked1Element(relation2Type)] = 0;
                                        }
                                        else
                                        {
                                            for (l = 0; l < linkedElementCount2; ++l) endElementDictionary[endElement1.getLinkedNElement(relation2Type, l)] = 0;
                                        }
                                    }
                                    else
                                    {
                                        endElementDictionary[endElement1] = 0;
                                    }
                                }

                                if (endElementDictionary.Count != 0)
                                {
                                    Text questionText = new Text();
                                    foreach (XmlQuestionText1P xmlQuestionText in xmlRelationNQuestionCategory.questionText)
                                    {
                                        string languageId = xmlQuestionText.language.ToString();
                                        string questionString = String.Format(xmlQuestionText.text, questionAttributeValue.Value.getText(languageId));
                                        questionText.setText(languageId, questionString);
                                    }
                                    if (_quizData.verifyText(questionText, String.Format("question {0}", questionNameInLog)) != 0) return -1;

                                    Choice ExcludedChoice = null;
                                    if (startElementType == endElementType && choiceDictionary.ContainsKey(startElement)) ExcludedChoice = choiceDictionary[startElement];

                                    question = new MultipleAnswerQuestion(questionText, ExcludedChoice, startElement, xmlRelationNQuestionCategory.answerProximityCriterion, ChoiceCommentModeEnum.ATTRIBUTE_COMMENT);

                                    foreach(Element endElement in endElementDictionary.Keys)
                                    {
                                        AttributeValue answerAttributeValue = endElement.getAttributeValue(answerAttributeType);
                                        if (answerAttributeValue != null && choiceDictionary.ContainsKey(endElement))
                                        {
                                            Choice choice = choiceDictionary[endElement];
                                            question.addChoice(choice);
                                        }
                                    }
                                }

                                if (question != null && question.getChoiceCount() != 0)
                                {
                                    category.addQuestion(question);
                                }
                            }
                        }

                        int choiceCount = category.ChoiceCount;
                        if (startElementType == endElementType) --choiceCount;

                        if (category.QuestionCount == 0)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Level \"{0}\": No question in category \"{1}\". The category is ignored",
                                _nameInLog, questionNameInLog));
                            _weightSum -= weight;
                        }
                        else if (choiceCount < _choiceCount)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                               "Level \"{0}\", category \"{1}\": Not enough choices ({2} choices, {3} required). The category is ignored",
                               _nameInLog, questionNameInLog, choiceCount, _choiceCount));
                            _weightSum -= weight;
                        }
                        else
                        {
                            if (xmlRelationNQuestionCategory.commentMode == XmlCommentModeEnum.QUESTION_ATTRIBUTE)
                            {
                                if (relation2Type == null) category.setComments(relationType, questionAttributeType);
                                else category.setComments(relationType, relation2Type, questionAttributeType);
                            }
                            else if (xmlRelationNQuestionCategory.commentMode == XmlCommentModeEnum.NAME)
                            {
                                if (relation2Type == null) category.setComments(relationType, null);
                                else category.setComments(relationType, relation2Type, null);
                            }

                            MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Level \"{0}\", category \"{1}\": {2} question(s), {3} choice(s)",
                                _nameInLog, questionNameInLog, category.QuestionCount, choiceCount));
                            _categoryList.Add(category);
                            _totalQuestionCount += category.QuestionCount;
                        }
                    }
                    else if (!_elementByTypeDictionary.ContainsKey(startElementType))
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                            "Level \"{0}\": No question in category \"{1}\" because there is no element of type {2}. The category is ignored",
                            _nameInLog, questionNameInLog, startElementType.id));
                    }
                    else
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                            "Level \"{0}\": No question in category \"{1}\" because there is no element of type {2}. The category is ignored",
                            _nameInLog, questionNameInLog, endElementType.id));
                    }
                }
            }

            return 0;
        }

        private int addAttributeOrderQuestions()
        {
            foreach (XmlAttributeOrderQuestionCategory xmlAttributeOrderQuestionCategory in _quizData.XmlQuizData.questionCategories.attributeOrderQuestionCategoryList)
            {
                Int32 minLevel = Int32.Parse(xmlAttributeOrderQuestionCategory.minLevel);
                if (_value >= minLevel && (!_testMode || xmlAttributeOrderQuestionCategory.test))
                {
                    XmlElementType elementType = _quizData.getXmlElementType(xmlAttributeOrderQuestionCategory.elementType);

                    string questionNameInLog = xmlAttributeOrderQuestionCategory.questionText[_logLanguageIndex].text;

                    if (_elementByTypeDictionary.ContainsKey(elementType))
                    {
                        XmlAttributeType answerAttributeType = _quizData.getXmlAttributeType(xmlAttributeOrderQuestionCategory.answerAttribute);
                        if (!answerAttributeType.canBeQuestion)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Error in {0}: Error in definition of category \"{1}\": Attribute {2} can not be used as an answer",
                                _quizData.DataFileName, questionNameInLog, answerAttributeType.id));
                            return -1;
                        }

                        XmlNumericalAttributeType numericalAttributeType = _quizData.getXmlNumericalAttributeType(xmlAttributeOrderQuestionCategory.attribute);
                        Int32 weight = Int32.Parse(xmlAttributeOrderQuestionCategory.weight);
                        _weightSum += weight;

                        Text questionText = new Text();
                        foreach (XmlQuestionText0P xmlQuestionText in xmlAttributeOrderQuestionCategory.questionText)
                        {
                            string languageId = xmlQuestionText.language.ToString();
                            questionText.setText(languageId, xmlQuestionText.text);
                        }
                        if (_quizData.verifyText(questionText, String.Format("question {0}", questionNameInLog)) != 0) return -1;

                        double distribParameterCorrection = 0.0;
                        if (xmlAttributeOrderQuestionCategory.distribParameterCorrectionSpecified) distribParameterCorrection = xmlAttributeOrderQuestionCategory.distribParameterCorrection;

                        MapParameters mapParameters = new MapParameters(xmlAttributeOrderQuestionCategory.mapParameters);
                        AttributeOrderCategory category = new AttributeOrderCategory(_weightSum, questionNameInLog, _quizData, numericalAttributeType, questionText,
                                                                                     xmlAttributeOrderQuestionCategory.mode, distribParameterCorrection, mapParameters);

                        foreach (Element element in _elementByTypeDictionary[elementType])
                        {
                            AttributeValue answerAttributeValue = element.getAttributeValue(answerAttributeType);
                            if (answerAttributeValue != null)
                            {
                                NumericalAttributeValue value = element.getNumericalAttributeValue(numericalAttributeType);
                                if (value != null) category.addAnswer(value.Value, element);
                            }
                        }

                        if (category.ChoiceCount < _choiceCount)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                                "Level \"{0}\", category \"{1}\": Not enough choices ({2} choices, {3} required). The category is ignored",
                                _nameInLog, questionNameInLog, category.ChoiceCount, _choiceCount));
                            _weightSum -= weight;
                        }
                        else
                        {
                            if (category.setElementVector(_choiceCount) != 0)
                            {
                                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                                        "Level \"{0}\": No question in category \"{1}\". The category is ignored", _nameInLog, questionNameInLog));
                                _weightSum -= weight;
                            }
                            else
                            {
                                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Level \"{0}\", category \"{1}\": 1 question, {2} choice(s)",
                                                         _nameInLog, questionNameInLog, category.ChoiceCount));
                                _categoryList.Add(category);
                                ++_totalQuestionCount;
                            }
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

        private int addRelationLimitQuestions()
        {
            foreach (XmlRelationLimitQuestionCategory xmlRelationLimitQuestionCategory in _quizData.XmlQuizData.questionCategories.relationLimitQuestionCategoryList)
            {
                Int32 minLevel = Int32.Parse(xmlRelationLimitQuestionCategory.minLevel);
                if (_value >= minLevel && (!_testMode || xmlRelationLimitQuestionCategory.test))
                {
                    RelationType relationType = _quizData.getRelationType(xmlRelationLimitQuestionCategory.relation);
                    if (xmlRelationLimitQuestionCategory.way == XmlWayEnum.INVERSE) relationType = relationType.ReciprocalType;

                    string questionNameInLog = xmlRelationLimitQuestionCategory.questionText[_logLanguageIndex].text;
                     
                    if (relationType.Nature == RelationNatureEnum.RELATION_11 || (relationType.Nature == RelationNatureEnum.RELATION_1N && relationType.Way != RelationWayEnum.DIRECT))
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format(
                               "Error in {0}: Error in definition of category \"{1}\": Relation ({2}) can not define \"relation limit\" questions",
                               _quizData.DataFileName, questionNameInLog, relationType.FullName));
                        return -1;
                    }

                    XmlElementType startElementType = relationType.StartType;
                    XmlElementType endElementType = relationType.EndType;

                    if (startElementType == endElementType)
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format(
                               "Error in {0}: Error in definition of category \"{1}\": Start element type is the same as end element type ({2}). (Start element type and end element type must be different for a \"relation limit\" category)",
                               _quizData.DataFileName, questionNameInLog, startElementType.id));
                        return -1;
                    }

                    if (_elementByTypeDictionary.ContainsKey(startElementType) && _elementByTypeDictionary.ContainsKey(endElementType))
                    {
                        XmlAttributeType questionAttributeType = _quizData.getXmlAttributeType(xmlRelationLimitQuestionCategory.questionAttribute);
                        if (!questionAttributeType.canBeQuestion)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Error in {0}: Error in definition of category \"{1}\": Attribute {2} can not be used as a question",
                                _quizData.DataFileName, questionNameInLog, questionAttributeType.id));
                            return -1;
                        }

                        XmlAttributeType answerAttributeType = _quizData.getXmlAttributeType(xmlRelationLimitQuestionCategory.answerAttribute);

                        double distribParameterCorrection = 0.0;
                        if (xmlRelationLimitQuestionCategory.distribParameterCorrectionSpecified) distribParameterCorrection = xmlRelationLimitQuestionCategory.distribParameterCorrection;

                        Int32 weight = Int32.Parse(xmlRelationLimitQuestionCategory.weight);
                        _weightSum += weight;

                        MapParameters mapParameters = new MapParameters(xmlRelationLimitQuestionCategory.mapParameters);
                        SimpleAnswerCategory category = new SimpleAnswerCategory(_weightSum, questionNameInLog, _quizData, xmlRelationLimitQuestionCategory.answerProximityCriterion,
                                                                                 distribParameterCorrection, mapParameters);
                        Dictionary<Element, Choice> choiceDictionary = new Dictionary<Element, Choice>();

                        foreach (Element endElement in _elementByTypeDictionary[endElementType])
                        {
                            AttributeValue answerAttributeValue = endElement.getAttributeValue(answerAttributeType);
                            if (answerAttributeValue != null)
                            {
                                if (xmlRelationLimitQuestionCategory.answerProximityCriterion != XmlSimpleAnswerProximityCriterionEnum.ELEMENT_LOCATION || endElement.GeoPoint != null)
                                {
                                    Choice choice = new Choice(answerAttributeValue, endElement, _quizData);
                                    category.addChoice(choice);
                                    choiceDictionary.Add(endElement, choice);
                                }
                            }
                        }

                        Dictionary<Element, int> startElementExceptionDictionary = new Dictionary<Element, int>();
                        if (xmlRelationLimitQuestionCategory.exception != null)
                        {
                            foreach(XmlException1 xmlException1 in xmlRelationLimitQuestionCategory.exception)
                            {
                                Element exceptionElement;
                                if (_elementDictionary.TryGetValue(xmlException1.questionElement, out exceptionElement))
                                {
                                    startElementExceptionDictionary.Add(exceptionElement, 0);
                                }
                            }
                        }

                       foreach (Element startElement in _elementByTypeDictionary[startElementType])
                       {
                            AttributeValue questionAttributeValue = startElement.getAttributeValue(questionAttributeType);
                            int endElementCount = startElement.getLinkedNElementCount(relationType);

                            if (questionAttributeValue != null && endElementCount != 0 && !startElementExceptionDictionary.ContainsKey(startElement))
                            {
                                Element endElement;
                                if (xmlRelationLimitQuestionCategory.mode == XmlRelationOrderModeEnum.BEGIN) endElement = startElement.getLinkedNElement(relationType, 0);
                                else endElement = startElement.getLinkedNElement(relationType, endElementCount - 1);

                                AttributeValue answerAttributeValue = endElement.getAttributeValue(answerAttributeType);
                                if (answerAttributeValue != null)
                                {
                                    Choice choice = choiceDictionary[endElement];

                                    Text questionText = new Text();
                                    foreach (XmlQuestionText1P xmlQuestionText in xmlRelationLimitQuestionCategory.questionText)
                                    {
                                        string languageId = xmlQuestionText.language.ToString();
                                        string questionString = String.Format(xmlQuestionText.text, questionAttributeValue.Value.getText(languageId));
                                        questionText.setText(languageId, questionString);
                                    }
                                    if (_quizData.verifyText(questionText, String.Format("question {0}", questionNameInLog)) != 0) return -1;

                                    SimpleAnswerQuestion question = new SimpleAnswerQuestion(questionText, choice, null, startElement, xmlRelationLimitQuestionCategory.answerProximityCriterion);
                                    category.addQuestion(question);
                                }
                            }
                        }

                        int choiceCount = category.ChoiceCount;

                        if (category.QuestionCount == 0)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Level \"{0}\": No question in category \"{1}\". The category is ignored",
                                _nameInLog, questionNameInLog));
                            _weightSum -= weight;
                        }
                        else if (choiceCount < _choiceCount)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                               "Level \"{0}\", category \"{1}\": Not enough choices ({2} choices, {3} required). The category is ignored",
                               _nameInLog, questionNameInLog, choiceCount, _choiceCount));
                            _weightSum -= weight;
                        }
                        else
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Level \"{0}\", category \"{1}\": {2} question(s), {3} choice(s)",
                                _nameInLog, questionNameInLog, category.QuestionCount, choiceCount));
                            _categoryList.Add(category);
                            _totalQuestionCount += category.QuestionCount;
                        }
                    }
                    else if (!_elementByTypeDictionary.ContainsKey(startElementType))
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                            "Level \"{0}\": No question in category \"{1}\" because there is no element of type {2}. The category is ignored",
                            _nameInLog, questionNameInLog, startElementType.id));
                    }
                    else
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                            "Level \"{0}\": No question in category \"{1}\" because there is no element of type {2}. The category is ignored",
                            _nameInLog, questionNameInLog, endElementType.id));
                    }
                }
            }

            return 0;
        }

        private int addRelationOrderQuestions()
        {
            foreach (XmlRelationOrderQuestionCategory xmlRelationOrderQuestionCategory in _quizData.XmlQuizData.questionCategories.relationOrderQuestionCategoryList)
            {
                int minLevel = Int32.Parse(xmlRelationOrderQuestionCategory.minLevel);
                if (_value >= minLevel && (!_testMode || xmlRelationOrderQuestionCategory.test))
                {
                    RelationType relationType = _quizData.getRelationType(xmlRelationOrderQuestionCategory.relation);
                    if (xmlRelationOrderQuestionCategory.way == XmlWayEnum.INVERSE) relationType = relationType.ReciprocalType;

                    string questionNameInLog = xmlRelationOrderQuestionCategory.questionText[_logLanguageIndex].text;

                    if (relationType.Nature == RelationNatureEnum.RELATION_11 || (relationType.Nature == RelationNatureEnum.RELATION_1N && relationType.Way != RelationWayEnum.DIRECT))
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format(
                               "Error in {0}: Error in definition of category \"{1}\": Relation ({2}) can not define \"relation order\" questions",
                               _quizData.DataFileName, questionNameInLog, relationType.FullName));
                        return -1;
                    }

                    XmlElementType startElementType = relationType.StartType;
                    XmlElementType endElementType = relationType.EndType;

                    if (_elementByTypeDictionary.ContainsKey(startElementType) && _elementByTypeDictionary.ContainsKey(endElementType))
                    {
                        XmlAttributeType questionAttributeType = _quizData.getXmlAttributeType(xmlRelationOrderQuestionCategory.questionAttribute);
                        if (!questionAttributeType.canBeQuestion)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Error in {0}: Error in definition of category \"{1}\": Attribute {2} can not be used as a question",
                                _quizData.DataFileName, questionNameInLog, questionAttributeType.id));
                            return -1;
                        }

                        XmlAttributeType answerAttributeType = _quizData.getXmlAttributeType(xmlRelationOrderQuestionCategory.answerAttribute);

                        double distribParameterCorrection = 0.0;
                        if (xmlRelationOrderQuestionCategory.distribParameterCorrectionSpecified) distribParameterCorrection = xmlRelationOrderQuestionCategory.distribParameterCorrection;

                        Int32 weight = Int32.Parse(xmlRelationOrderQuestionCategory.weight);
                        _weightSum += weight;

                        MapParameters mapParameters = new MapParameters(xmlRelationOrderQuestionCategory.mapParameters);
                        RelationOrderCategory category = new RelationOrderCategory(_weightSum, questionNameInLog, _quizData, distribParameterCorrection, mapParameters);

                        foreach (Element startElement in _elementByTypeDictionary[startElementType])
                        {
                            AttributeValue questionAttributeValue = startElement.getAttributeValue(questionAttributeType);

                            int endElementCount = startElement.getLinkedNElementCount(relationType);

                            if (questionAttributeValue != null && endElementCount != 0 && endElementCount >= 2 * _choiceCount)
                            {
                                Text questionText = new Text();
                                foreach (XmlQuestionText1P xmlQuestionText in xmlRelationOrderQuestionCategory.questionText)
                                {
                                    string languageId = xmlQuestionText.language.ToString();
                                    string questionString = String.Format(xmlQuestionText.text, questionAttributeValue.Value.getText(languageId));
                                    questionText.setText(languageId, questionString);
                                }
                                if (_quizData.verifyText(questionText, String.Format("question {0}", questionNameInLog)) != 0) return -1;

                                RelationOrderQuestion question = new RelationOrderQuestion(questionText, startElement);

                                int k1, k2, dk;
                                if (xmlRelationOrderQuestionCategory.mode == XmlRelationOrderModeEnum.BEGIN)
                                {
                                    k1 = 0;
                                    k2 = endElementCount;
                                    dk = 1;
                                }
                                else
                                {
                                    k1 = endElementCount - 1;
                                    k2 = -1;
                                    dk = -1;
                                }

                                int k;
                                for (k = k1; k != k2; k += dk)
                                {
                                    Element linkedElement = startElement.getLinkedNElement(relationType, k);
                                    AttributeValue answerAttributeValue = linkedElement.getAttributeValue(answerAttributeType);
                                    if (answerAttributeValue != null)
                                    {
                                        Choice choice = new Choice(answerAttributeValue, linkedElement, _quizData);
                                        question.addChoice(choice);
                                    }
                                }    

                                if (question.ChoiceCount >= 2 * _choiceCount)
                                {
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
                        else
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Level \"{0}\", category \"{1}\": {2} question(s)",
                                _nameInLog, questionNameInLog, category.QuestionCount));
                            _categoryList.Add(category);
                            _totalQuestionCount += category.QuestionCount;
                        }
                    }
                    else if (!_elementByTypeDictionary.ContainsKey(startElementType))
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                            "Level \"{0}\": No question in category \"{1}\" because there is no element of type {2}. The category is ignored",
                            _nameInLog, questionNameInLog, startElementType.id));
                    }
                    else
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                            "Level \"{0}\": No question in category \"{1}\" because there is no element of type {2}. The category is ignored",
                            _nameInLog, questionNameInLog, endElementType.id));
                    }
                }
            }

            return 0;
        }

        private int addRelationExistenceQuestions()
        {
            foreach (XmlRelationExistenceQuestionCategory xmlRelationExistenceQuestionCategory in _quizData.XmlQuizData.questionCategories.relationExistenceQuestionCategoryList)
            {
                int minLevel = Int32.Parse(xmlRelationExistenceQuestionCategory.minLevel);
                if (_value >= minLevel && (!_testMode || xmlRelationExistenceQuestionCategory.test))
                {
                    RelationType relationType = _quizData.getRelationType(xmlRelationExistenceQuestionCategory.relation);
                    if (xmlRelationExistenceQuestionCategory.way == XmlWayEnum.INVERSE) relationType = relationType.ReciprocalType;
                    XmlElementType elementType = relationType.StartType;
                    string questionNameInLog = xmlRelationExistenceQuestionCategory.questionText[_logLanguageIndex].text;

                    Dictionary<Element, int> elementExceptionDictionary = new Dictionary<Element, int>();
                    if (xmlRelationExistenceQuestionCategory.exception != null)
                    {
                        foreach (XmlException1 xmlException1 in xmlRelationExistenceQuestionCategory.exception)
                        {
                            Element exceptionElement;
                            if (_elementDictionary.TryGetValue(xmlException1.questionElement, out exceptionElement))
                            {
                                elementExceptionDictionary.Add(exceptionElement, 0);
                            }
                        }
                    }

                    Text questionText = new Text();
                    foreach (XmlQuestionText0P xmlQuestionText in xmlRelationExistenceQuestionCategory.questionText)
                    {
                        string languageId = xmlQuestionText.language.ToString();
                        questionText.setText(languageId, xmlQuestionText.text);
                    }
                    if (_quizData.verifyText(questionText, String.Format("question {0}", questionNameInLog)) != 0) return -1;

                    if (_elementByTypeDictionary.ContainsKey(elementType))
                    {
                        XmlAttributeType answerAttributeType = _quizData.getXmlAttributeType(xmlRelationExistenceQuestionCategory.answerAttribute);

                        Int32 weight = Int32.Parse(xmlRelationExistenceQuestionCategory.weight);
                        _weightSum += weight;

                        MapParameters mapParameters = new MapParameters(xmlRelationExistenceQuestionCategory.mapParameters);
                        MultipleAnswerCategory category = new MultipleAnswerCategory(_weightSum, questionNameInLog, _quizData, XmlMultipleAnswerProximityCriterionEnum.NONE, 0.0, mapParameters);
                        MultipleAnswerQuestion question = new MultipleAnswerQuestion(questionText, null, null, XmlMultipleAnswerProximityCriterionEnum.NONE, ChoiceCommentModeEnum.MAIN_COMMENT);

                        foreach (Element element in _elementByTypeDictionary[elementType])
                        {
                            AttributeValue answerAttributeValue = element.getAttributeValue(answerAttributeType);
                            if (answerAttributeValue != null && !elementExceptionDictionary.ContainsKey(element))
                            {
                                Choice choice = new Choice(answerAttributeValue, element, _quizData);
                                category.addChoice(choice);

                                if (xmlRelationExistenceQuestionCategory.commentMode == XmlCommentModeEnum.NAME)
                                {
                                    Dictionary<Element, int> linkedElements = new Dictionary<Element, int>();

                                    Element linkedElement1 = element.getLinked1Element(relationType);
                                    if (linkedElement1 != null) linkedElements.Add(linkedElement1, 0);

                                    int i, n = element.getLinkedNElementCount(relationType);
                                    for (i = 0; i < n; ++i)
                                    {
                                        Element linkedElementN = element.getLinkedNElement(relationType, i);
                                        if (!linkedElements.ContainsKey(linkedElementN)) linkedElements.Add(linkedElementN, 0);
                                    }

                                    List<Text> textList = new List<Text>();
                                    foreach (Element elt in linkedElements.Keys) textList.Add(elt.Name);
                                    choice.Comment = Text.fromTextList(textList, _quizData);
                                }

                                bool relationExists = (element.getLinked1Element(relationType) != null || element.getLinkedNElementCount(relationType) != 0);
                                if (relationExists == (xmlRelationExistenceQuestionCategory.mode == XmlRelationExistenceModeEnum.EXISTS)) question.addChoice(choice);
                            }
                        }

                        if (question.getChoiceCount() == 0)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Level \"{0}\": No question in category \"{1}\". The category is ignored",
                                _nameInLog, questionNameInLog));
                            _weightSum -= weight;
                        }
                        else if (category.ChoiceCount < _choiceCount)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                               "Level \"{0}\", category \"{1}\": Not enough choices ({2} choices, {3} required). The category is ignored",
                               _nameInLog, questionNameInLog, question.getChoiceCount(), _choiceCount));
                            _weightSum -= weight;
                        }
                        else
                        {
                            category.addQuestion(question);
                            MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Level \"{0}\", category \"{1}\": {2} question(s), {3} choice(s)",
                                _nameInLog, questionNameInLog, category.QuestionCount, category.ChoiceCount));
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
                { "level_id", _xmlLevel.levelId },
                { "index", _value },
                { "name", _name.getBsonDocument() },
                { "question_count", _questionCount },
                { "choice_count", _choiceCount },
                { "weight_sum", _weightSum },
                { "distrib_parameter", _xmlLevel.distribParameter },
                { "category_count", _categoryList.Count }
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

        internal int generateCode()
        {
            MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("level \"{0}\": C++ code generation...", _nameInLog));

            List<CodeGenerator> codeGeneratorList = new List<CodeGenerator>();

            foreach (string languageId in Text.CompletedTranslationLanguages)
            {
                string dirName = String.Format("{0}/{1}{2}{3}", _quizData.XmlQuizData.parameters.cppGenerationDir, _quizData.XmlQuizData.parameters.questionnaireId,
                                               _xmlLevel.levelId, languageId);

                codeGeneratorList.Add(new CodeGenerator(dirName, languageId));
            }

            foreach (Category category in _categoryList)
            {
                if (category.generateCode(codeGeneratorList) != 0) return -1;
            }

            foreach (CodeGenerator generator in codeGeneratorList) generator.close(_questionCount, _weightSum, _xmlLevel.distribParameter, _choiceCount);

            return 0;
        }
    }
}
