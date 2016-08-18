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
        private Int32 _questionCount;
        private Int32 _totalQuestionCount = 0;
        private readonly Dictionary<string, Element> _elementDictionary = new Dictionary<string, Element>();
        private readonly Dictionary<XmlElementType, List<Element>> _elementByTypeDictionary = new Dictionary<XmlElementType, List<Element>>();
        private readonly List<Category> _categoryList = new List<Category>();

        internal int initialize(QuizData quizData, XmlLevel xmlLevel)
        {
            _xmlLevel = xmlLevel;
            _quizData = quizData;

            _nameInLog = _xmlLevel.name[0].text;
            foreach (XmlName xmlName in _xmlLevel.name) _name.setText(xmlName.language.ToString(), xmlName.text);
            quizData.verifyText(_name, String.Format("Level {0}", _nameInLog));

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

            /*if (!addAttributeOrderQuestions()) return false;
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

        private int addAttributeQuestions()
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
                        SimpleAnswerCategory category = new SimpleAnswerCategory(_weightSum, questionNameInLog, _quizData, xmlAttributeQuestionCategory.answerProximityCriterion, distribParameterCorrection);

                        foreach (Element element in _elementByTypeDictionary[elementType])
                        {
                            AttributeValue questionAttributeValue = element.getAttributeValue(questionAttributeType);
                            AttributeValue answerAttributeValue = element.getAttributeValue(answerAttributeType);

                            if (answerAttributeValue != null) 
                            {
                                if (xmlAttributeQuestionCategory.answerProximityCriterion != XmlAnswerProximityCriterionEnum.ELEMENT_LOCATION || element.GeoPoint != null)
                                {
                                    Choice choice = new Choice(answerAttributeValue, element, _quizData);
                                    category.addChoice(choice);

                                    if (questionAttributeValue != null)
                                    {
                                        Text questionText = new Text();
                                        foreach (XmlQuestionText xmlQuestionText in xmlAttributeQuestionCategory.questionText)
                                        {
                                            string languageId = xmlQuestionText.language.ToString();
                                            string questionString = String.Format(xmlQuestionText.text, questionAttributeValue.Value.getText(languageId));
                                            questionText.setText(languageId, questionString);
                                        }
                                        if (_quizData.verifyText(questionText, String.Format("question {0}", questionNameInLog)) != 0) return -1;

                                        SimpleAnswerQuestion question = new SimpleAnswerQuestion(questionText, choice, null/*, null*/, xmlAttributeQuestionCategory.answerProximityCriterion);
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
                if (_value >= minLevel)
                {
                    RelationType relationType = _quizData.getRelationType(xmlRelation1QuestionCategory.relation);
                    if (xmlRelation1QuestionCategory.way == XmlWayEnum.INVERSE) relationType = relationType.ReciprocalType;

                    string questionNameInLog = xmlRelation1QuestionCategory.questionText[0].text;

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

                        SimpleAnswerCategory category = new SimpleAnswerCategory(_weightSum, questionNameInLog, _quizData, xmlRelation1QuestionCategory.answerProximityCriterion, distribParameterCorrection);
                        Dictionary<Element, Choice> choiceDictionary = new Dictionary<Element, Choice>();

                        foreach (Element endElement in _elementByTypeDictionary[endElementType])
                        {
                            AttributeValue answerAttributeValue = endElement.getAttributeValue(answerAttributeType);
                            if (answerAttributeValue != null)
                            {
                                if (xmlRelation1QuestionCategory.answerProximityCriterion != XmlAnswerProximityCriterionEnum.ELEMENT_LOCATION || endElement.GeoPoint != null)
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
                                        foreach (XmlQuestionText xmlQuestionText in xmlRelation1QuestionCategory.questionText)
                                        {
                                            string languageId = xmlQuestionText.language.ToString();
                                            string questionString = String.Format(xmlQuestionText.text, questionAttributeValue.Value.getText(languageId));
                                            questionText.setText(languageId, questionString);
                                        }
                                        if (_quizData.verifyText(questionText, String.Format("question {0}", questionNameInLog)) != 0) return -1;

                                        Choice ExcludedChoice = null;
                                        if (startElementType == endElementType) ExcludedChoice = choiceDictionary[startElement];

                                        SimpleAnswerQuestion question = new SimpleAnswerQuestion(questionText, choice, ExcludedChoice/*, startElement*/, xmlRelation1QuestionCategory.answerProximityCriterion);
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
                if (_value >= minLevel)
		        {
                    RelationType relationType = _quizData.getRelationType(xmlRelationNQuestionCategory.relation);
                    if (xmlRelationNQuestionCategory.way == XmlWayEnum.INVERSE) relationType = relationType.ReciprocalType;

                    string questionNameInLog = xmlRelationNQuestionCategory.questionText[0].text;

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

				        /*const wchar_t *answerAttr = XmlFunctions::get_relationNQuestionCategory_answerAttribute (i);
				        const AttributeType *answerAttributeType = attributeTypeMap[answerAttr];

				        const wchar_t *criterion = XmlFunctions::get_relationNQuestionCategory_answerProximityCriterion (i);
				        ProximityCriterionEnum proximityCriterion;
				        if (criterion[0] == 'a') proximityCriterion = ATTRIBUTE_VALUE;
				        else if (criterion[0] == 's') proximityCriterion = SORT_KEY;
				        else if (criterion[0] == 'e') proximityCriterion = ELEMENT_LOCATION;
				        else proximityCriterion = NO_CRITERION;

				        int questionDrawDepth1 = XmlFunctions::get_relationNQuestionCategory_questionDrawDepth1 (i);
				        int answerDrawDepth1 = XmlFunctions::get_relationNQuestionCategory_answerDrawDepth1 (i);
				        int wrongChoiceDrawDepth1 = XmlFunctions::get_relationNQuestionCategory_wrongChoiceDrawDepth1 (i);
				        int questionDrawDepth2 = XmlFunctions::get_relationNQuestionCategory_questionDrawDepth2 (i);
				        int answerDrawDepth2 = XmlFunctions::get_relationNQuestionCategory_answerDrawDepth2 (i);
				        int wrongChoiceDrawDepth2 = XmlFunctions::get_relationNQuestionCategory_wrongChoiceDrawDepth2 (i);
				        double distribParameterCorrection = XmlFunctions::get_relationNQuestionCategory_distribParameterCorrection (i);

				        int weight = XmlFunctions::get_relationNQuestionCategory_weight (i);
				        _weightSum += weight;

				        MultipleAnswerCategory *category = new MultipleAnswerCategory (_weightSum, proximityCriterion, startElementType, endElementType, distribParameterCorrection, wrongChoiceDrawDepth1, wrongChoiceDrawDepth2);
				        std::map<const Element *, const PossibleAnswer *> possibleAnswerMap;

				        std::vector<const Element *> endElementVector = (*endEltIt).second;
				        int elementCount = endElementVector.size();
				        int j;
				        for (j=0; j<elementCount; ++j)
				        {
					        const Element *endElement = endElementVector[j];
					        const AttributeValue *answerAttributeValue = endElement->getAttributeValue (answerAttributeType);
					        if (answerAttributeValue != 0)
					        {
						        const PossibleAnswer *answer = new PossibleAnswer (answerAttributeValue, endElement);
						        category->addAnswer (answer);
						        possibleAnswerMap.insert (std::pair<const Element *, const PossibleAnswer *>(endElement, answer));
					        }
				        }
				
				        std::vector<const Element *> startElementVector = (*startEltIt).second;
				        elementCount = startElementVector.size();
				        for (j=0; j<elementCount; ++j)
				        {
					        const Element *startElement = startElementVector[j];
					        const AttributeValue *questionAttributeValue = startElement->getAttributeValue (questionAttributeType);

					        if (questionAttributeValue != 0)
					        {
						        MultipleAnswerQuestion *question = 0;
						        std::set<const Element *> mapElementSet1, mapElementSet2;
                                std::set<const Element *> mainMapElementSet;
						        std::set<const Element *> endElementSet;
						        int k, linkedElementCount1 = startElement->getLinkedNElementCount (relationType);
						        bool relation1Is1 = false;
						        if (linkedElementCount1 == 0 && startElement->getLinked1Element (relationType) != 0)
						        {
							        relation1Is1 = true;
							        linkedElementCount1 = 1;
						        }

						        for (k=0; k<linkedElementCount1; ++k)
						        {
							        const Element *endElement1;
							        if (relation1Is1) endElement1 = startElement->getLinked1Element (relationType);
							        else endElement1 = startElement->getLinkedNElement (relationType, k);
							
							        if (rel2Type[0] != 0)
							        {
								        int l, linkedElementCount2 = endElement1->getLinkedNElementCount (relation2Type);
								        if (linkedElementCount2 == 0 && endElement1->getLinked1Element (relation2Type) != 0)
								        {
									        endElementSet.insert (endElement1->getLinked1Element (relation2Type));
								        }
								        else
								        {
									        for (l=0; l<linkedElementCount2; ++l) endElementSet.insert (endElement1->getLinkedNElement (relation2Type, l));
								        }
							        }
							        else
							        {
								        endElementSet.insert (endElement1);
							        }
						        }
			
						        if (endElementSet.size() != 0)
						        {
							        std::wstring questionText (questionTemplate);
							        questionText.replace (questionText.find ('%'), 1, questionAttributeValue->getValue());
							        question = new MultipleAnswerQuestion (questionText, startElement);
							        startElement->getMapElements (0, mapElementSet2, questionDrawDepth1, 0);
                                    std::set<const Element *>::iterator it = mapElementSet2.begin();
	                                for (; it!=mapElementSet2.end(); ++it) mapElementSet1.insert (*it);
                                    mapElementSet2.clear();
							        startElement->getMapElements (1, mapElementSet2, questionDrawDepth2, 0);
                                    for (it=mapElementSet2.begin(); it!=mapElementSet2.end(); ++it) mapElementSet1.insert (*it);
                                    mapElementSet2.clear();
                                    mainMapElementSet.insert (startElement);

							        std::set<const Element *>::iterator eltIt = endElementSet.begin();
							        for (; eltIt!=endElementSet.end(); ++eltIt)
							        {
								        const Element *endElement = (*eltIt);
								        const AttributeValue *answerAttributeValue = endElement->getAttributeValue (answerAttributeType);
								        if (answerAttributeValue != 0)
								        {
									        const PossibleAnswer *answer = possibleAnswerMap[endElement];
									        endElement->getMapElements (0, mapElementSet2, answerDrawDepth1, 0);
                                            for (it=mapElementSet2.begin(); it!=mapElementSet2.end(); ++it) mapElementSet1.insert (*it);
                                            mapElementSet2.clear();
									        endElement->getMapElements (1, mapElementSet2, answerDrawDepth2, 0);
                                            for (it=mapElementSet2.begin(); it!=mapElementSet2.end(); ++it) mapElementSet1.insert (*it);
                                            mapElementSet2.clear();
									        question->addAnswer (answer);
								        }
							        }	
						        }	

						        if (question != 0 && question->getAnswerCount() != 0)
						        {
							        question->addMapElements (mapElementSet1, mainMapElementSet);
							        category->addQuestion (question);
						        }
						        else
						        {
							        delete question;
						        }
					        }
				        }

				        unsigned int possibleAnswerCount = category->getDistinctAnswerCount();
				        if (startElementType == endElementType) --possibleAnswerCount;

				        if (category->getQuestionCount() == 0)
				        {
					        emptyCategoryWarning2 (questionTemplate);
					        _weightSum -= weight;
					        delete category;
				        }
				        else if (possibleAnswerCount < _choiceCount)
				        {
					        notEnoughAnswerWarning (possibleAnswerCount, questionTemplate);
					        _weightSum -= weight;
					        delete category;
				        }
				        else
				        {
                            const wchar_t *commentMode = XmlFunctions::get_relationNQuestionCategory_commentMode (i);
                            if (commentMode[0] == 'q')
                            {
                                if (relation2Type == 0) category->setComments (relationType, questionAttributeType);
                                else category->setComments (relationType, relation2Type, questionAttributeType);
                            }
                            else if (commentMode[1] == 'a')
                            {
                                if (relation2Type == 0) category->setComments (relationType, 0);
                                else category->setComments (relationType, relation2Type, 0);
                            }

					        questionCountInfoMessage1 (category->getQuestionCount(), possibleAnswerCount, questionTemplate);
					        _categoryVector.push_back (category);
					        _totalQuestionCount += category->getQuestionCount();
				        }*/
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
    }
}
