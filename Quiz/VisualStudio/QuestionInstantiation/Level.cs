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
        private Int32 _value;
        private Int32 _weightSum = 0;
        private Int32 _choiceCount;
        private readonly Dictionary<string, Element> _elementDictionary = new Dictionary<string, Element>();
        private readonly Dictionary<XmlElementType, List<Element>> _elementByTypeDictionary = new Dictionary<XmlElementType, List<Element>>();
        private readonly List<Category> _categoryList = new List<Category>();

        internal int initialize(QuizData quizData, XmlLevel xmlLevel)
        {
            _xmlLevel = xmlLevel;
            _value = Int32.Parse(_xmlLevel.value);
            _choiceCount = Int32.Parse(_xmlLevel.choiceCount);

            if (addElements(quizData) != 0) return -1;
            if (checkSymetricalRelations(quizData) != 0) return -1;
            if (addQuestions(quizData) != 0) return -1;

            return 0;
        }

        private int addElements(QuizData quizData)
        {
            Dictionary<string, List<Element>> elementByAttributeKeyDictionary = new Dictionary<string, List<Element>>();

            foreach (XmlElement xmlElement in quizData.XmlQuizData.elementList)
	        {
                Int32 minLevel = Int32.Parse(xmlElement.minLevel);
                if (_value >= minLevel)
		        {
                    XmlElementType xmlElementType = quizData.getXmlElementType(xmlElement.type);
                    Element element = new Element(xmlElement, xmlElementType);

                    if (element.addAttributes(quizData) != 0) return -1;
                    if (element.addNumericalAttributes(quizData) != 0) return -1;

                    _elementDictionary.Add(xmlElement.id, element);

			        foreach (string attributeKey in element.AttributeKeyList)
			        {
                        if (!elementByAttributeKeyDictionary.ContainsKey(attributeKey)) elementByAttributeKeyDictionary.Add(attributeKey, new List<Element>());
                        elementByAttributeKeyDictionary[attributeKey].Add(element);
			        }
		        }
	        }

	        foreach (KeyValuePair<string, List<Element>> pair in elementByAttributeKeyDictionary)
	        {
                List<Element> elementList = pair.Value;

                if (elementList.Count > 1)
		        {
                    string key = pair.Key;
                    string eltTypeStr = key.Substring(0, key.IndexOf('+'));
                    string attrTypeStr = key.Substring(key.IndexOf('+') + 1);
                    string valueStr = attrTypeStr.Substring(attrTypeStr.IndexOf('+') + 1);
                    attrTypeStr = attrTypeStr.Substring(0, attrTypeStr.IndexOf('+')); 

                    foreach (Element element in elementList)
			        {			
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                            "Level \"{0}\": Element {1} has the same value ({2}) for attribute {3} as another element of same type ({4}). Since the attribute type {3} can be used as a question, the element {1} is ignored to avoid ambiguous questions",
                            _xmlLevel.name, element.XmlElement.id, valueStr, attrTypeStr, eltTypeStr));

                        _elementDictionary.Remove(element.XmlElement.id);
			        }
		        }
	        }

            foreach (KeyValuePair<string, Element> pair in _elementDictionary)
	        {
                Element element = pair.Value;

                if (element.addRelations(quizData, _elementDictionary) == 0)
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

        int checkSymetricalRelations(QuizData quizData)
        {
            foreach (RelationType relationType in quizData.RelationTypeDictionary.Values)
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
                                _xmlLevel.name, relationType.Id, element1.XmlElement.id, element2.XmlElement.id);

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

        int addQuestions(QuizData quizData)
        {
            if (addAttributeQuestions(quizData) != 0) return -1;

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

        int addAttributeQuestions(QuizData quizData)
        {
            foreach(XmlAttributeQuestionCategory xmlAttributeQuestionCategory in quizData.XmlQuizData.questionCategories.attributeQuestionCategoryList)
	        {
		        Int32 minLevel = Int32.Parse(xmlAttributeQuestionCategory.minLevel);
                if (_value >= minLevel)
		        {
                    XmlElementType elementType = quizData.getXmlElementType(xmlAttributeQuestionCategory.elementType);

                    string questionTemplate = xmlAttributeQuestionCategory.question;

			        if (_elementByTypeDictionary.ContainsKey(elementType))
			        {
                        XmlAttributeType questionAttributeType = quizData.getXmlAttributeType(xmlAttributeQuestionCategory.questionAttribute);
                        if (!questionAttributeType.canBeQuestion)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Error in {0}: Error in definition of category \"{1}\": Attribute {2} can not be used as a question",
                                quizData.DataFileName, xmlAttributeQuestionCategory.question, questionAttributeType.id));
                            return -1;
                        }

				        XmlAttributeType answerAttributeType = quizData.getXmlAttributeType(xmlAttributeQuestionCategory.answerAttribute);
				        Int32 weight = Int32.Parse(xmlAttributeQuestionCategory.weight);
                        _weightSum += weight;

                        SimpleAnswerCategory category = new SimpleAnswerCategory(_weightSum, xmlAttributeQuestionCategory.answerProximityCriterion, xmlAttributeQuestionCategory.distribParameterCorrection);

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
                                    string questionText = String.Format(questionTemplate, questionAttributeValue.Value);
                                    SimpleAnswerQuestion question = new SimpleAnswerQuestion(questionText, answer, null);
							        category.addQuestion(question);
						        }
					        }
				        }

				        if (category.QuestionCount == 0)
				        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Level \"{0}\": No question in category \"{1}\". The category is ignored",
                                _xmlLevel.name, questionTemplate));
					        _weightSum -= weight;
				        }
                        else if (category.DistinctAnswerCount < _choiceCount)
				        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                                "Level \"{0}\", category \"{1}\": Not enough possible answers ({2} possibles answers, {3} required). The category is ignored",
                                _xmlLevel.name, questionTemplate, category.DistinctAnswerCount, _choiceCount));
					        _weightSum -= weight;
				        }
				        else
				        {
                            /*const wchar_t *commentMode = XmlFunctions::get_attributeQuestionCategory_commentMode (i);
                            if (commentMode[0] == 'q') category->setComments (questionAttributeType);
                            else if (commentMode[1] == 'a') category->setComments (0);*/
                            if (xmlAttributeQuestionCategory.commentMode == XmlCommentModeEnum.QUESTION_ATTRIBUTE) category.setComments(questionAttributeType);
                            else if (xmlAttributeQuestionCategory.commentMode == XmlCommentModeEnum.NAME) category.setComments(null);

                            /*questionCountInfoMessage1 (category->getQuestionCount(), category->getDistinctAnswerCount(), questionTemplate);
					        _categoryVector.push_back (category);
					        _totalQuestionCount += category->getQuestionCount();*/
                        }
			        }
			        else
			        {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                                "Level \"{0}\": No question in category \"{1}\" because there is no element of type {2}. The category is ignored",
                                _xmlLevel.name, questionTemplate, elementType.id));
                    }
		        }
	        }

            return 0;
        }
    }
}
