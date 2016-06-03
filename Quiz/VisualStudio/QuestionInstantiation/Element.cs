using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class Element
    {
        private XmlElement _xmlElement;
        private XmlElementType _type;
        private Dictionary<XmlAttributeType, AttributeValue> _attributeDictionary = new Dictionary<XmlAttributeType,AttributeValue>();
        private List<string> _attributeKeyList = new List<string>();
        private Dictionary<XmlNumericalAttributeType, NumericalAttributeValue> _numericalAttributeDictionary = new Dictionary<XmlNumericalAttributeType, NumericalAttributeValue>();

        internal Element(XmlElement xmlElement, XmlElementType type)
        {
            _xmlElement = xmlElement;
            _type = type;
        }

        internal XmlElement XmlElement
        {
            get { return _xmlElement; }
        }

        internal List<string> AttributeKeyList
        {
            get { return _attributeKeyList; }
        }

        internal int addAttributes(QuizData quizData)
        {
            foreach (XmlAttribute xmlAttribute in _xmlElement.attributeList)
	        {
		        XmlAttributeType xmlAttributeType = quizData.getAttributeType(xmlAttribute.type);
                AttributeValue attributeValue = new AttributeValue(xmlAttribute.value, xmlAttribute.comment, xmlAttributeType);
                bool error = _attributeDictionary.ContainsKey(xmlAttributeType);
		      
		        if (error)
		        {
			        MessageLogger.addMessage(LogLevelEnum.FATAL_ERROR,
                        String.Format("Error in {0}: Attribute {1} is defined several times for element {2}",
                        quizData.DataFileName, xmlAttributeType.id, _xmlElement.id));
			        return -1;
		        }

                _attributeDictionary.Add(xmlAttributeType, attributeValue);

		        if (xmlAttributeType.canBeQuestion)
		        {
                    _attributeKeyList.Add(String.Format("{0}+{1}+{2}", _type.id, xmlAttributeType.id, attributeValue.Value));
		        }
	        }

            return 0;
        }

        internal int addNumericalAttributes(QuizData quizData)
        {
            foreach (XmlNumericalAttribute xmlNumericalAttribute in _xmlElement.numericalAttributeList)
	        {
                XmlNumericalAttributeType xmlNumericalAttributeType = quizData.getNumericalAttributeType(xmlNumericalAttribute.type);
                NumericalAttributeValue numericalAttributeValue = new NumericalAttributeValue(xmlNumericalAttribute.value, xmlNumericalAttributeType);
                bool error = _numericalAttributeDictionary.ContainsKey(xmlNumericalAttributeType);
                
                if (error)
                {
                    MessageLogger.addMessage(LogLevelEnum.FATAL_ERROR,
                        String.Format("Error in {0}: Numerical attribute {1} is defined several times for element {2}",
                        quizData.DataFileName, xmlNumericalAttributeType.id, _xmlElement.id));
                    return -1;
                }

                _numericalAttributeDictionary.Add(xmlNumericalAttributeType, numericalAttributeValue);
            }

            return 0;
        }

        internal int addRelations(QuizData quizData, Dictionary<string, Element> elementDictionary)
        {
            foreach (XmlRelation xmlRelation in _xmlElement.relationList)
	        {
                // If the linked element is in the current level...
                if (elementDictionary.ContainsKey(xmlRelation.linkedElement))
		        {
                    Element linkedElement = elementDictionary[xmlRelation.linkedElement];

			        /*const wchar_t *typeId = XmlFunctions::get_element_relation_type (elementIndex, i);
			        const RelationType *relationType = relationTypeMap[typeId];

			        if (_type != relationType->getStartType() || linkedElement->_type != relationType->getEndType())
			        {
				        const wchar_t *cMessage1 = XmlFunctions::get_errorMessages_typeMismatch1();
				        const wchar_t *cMessage2 = XmlFunctions::get_errorMessages_typeMismatch2();
				        std::wstring message1 (cMessage1);
				        std::wstring message2 (cMessage2);
				        message1.replace (message1.find ('%'), 1, _id);
				        message1.replace (message1.find ('#'), 1, linkedElement->_id);
				        if (_type != relationType->getStartType()) message2.replace (message2.find ('%'), 1, _id);
				        else message2.replace (message2.find ('%'), 1,linkedElement->_id);
				        message2.replace (message2.find ('#'), 1, relationType->getId());
				        message1.append (message2);
				        ErrorMessage::setMessage (message1);
				        MessageLogger::instance()->addMessage (FATAL_ERROR, message1.c_str());
				        return false;
			        }

			        if (relationType->getNature() == RELATION_11)
			        {
				        if (!addRelation1 (linkedElement, relationType)) return false;
				        if (!linkedElement->addRelation1 (this, relationType->getReciprocalType())) return false;
			        }
			        else if (relationType->getNature() == RELATION_1N)
			        {
				        if (!addRelationN (linkedElement, relationType)) return false;
				        if (!linkedElement->addRelation1 (this, relationType->getReciprocalType())) return false;
			        }
			        else
			        {
				        if (!addRelationN (linkedElement, relationType)) return false;
				        if (!linkedElement->addRelationN (this, relationType->getReciprocalType())) return false;
			        }*/	
		        }
	        }

            return 0;
        }
    }
}
