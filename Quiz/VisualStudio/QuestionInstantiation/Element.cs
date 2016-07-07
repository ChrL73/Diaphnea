using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class Element
    {
        private readonly XmlElement _xmlElement;
        private readonly XmlElementType _type;
        private readonly Text _name = new Text();
        private readonly Dictionary<XmlAttributeType, AttributeValue> _attributeDictionary = new Dictionary<XmlAttributeType, AttributeValue>();
        private readonly List<Text> _attributeKeyList = new List<Text>();
        private readonly Dictionary<XmlNumericalAttributeType, NumericalAttributeValue> _numericalAttributeDictionary = new Dictionary<XmlNumericalAttributeType, NumericalAttributeValue>();
        private readonly Dictionary<RelationType, Element> _relation1Dictionary = new Dictionary<RelationType, Element>();
        private readonly Dictionary<RelationType, List<Element>> _relationNDictionary = new Dictionary<RelationType, List<Element>>();

        internal Element(XmlElement xmlElement, XmlElementType type)
        {
            _xmlElement = xmlElement;
            _type = type;
        }

        internal int setName(QuizData quizData)
        {
            foreach (XmlName elementName in _xmlElement.elementName) _name.setText(elementName.language.ToString(), elementName.text);
            if (quizData.verifyText(_name, String.Format("name of element {0}", _xmlElement.id)) != 0) return -1;
            return 0;
        }

        internal Text Name { get { return _name; } }

        internal XmlElement XmlElement
        {
            get { return _xmlElement; }
        }

        internal XmlElementType Type
        {
            get { return _type; }
        }

        internal List<Text> AttributeKeyList
        {
            get { return _attributeKeyList; }
        }

        internal AttributeValue getAttributeValue(XmlAttributeType type)
        {
            AttributeValue value;
            if (_attributeDictionary.TryGetValue(type, out value)) return value;
            return null;
        }

        internal int getLinkedNElementCount(RelationType relationType)
        {
            if (_relationNDictionary.ContainsKey(relationType)) return _relationNDictionary[relationType].Count;
            return 0;
        }

        internal Element getLinkedNElement(RelationType relationType, int i)
        {
            return _relationNDictionary[relationType][i];
        }

        internal int addAttributes(QuizData quizData)
        {
            foreach (XmlAttribute xmlAttribute in _xmlElement.attributeList)
            {
                XmlAttributeType xmlAttributeType = quizData.getXmlAttributeType(xmlAttribute.type);

                Text valueText = new Text();
                Text commentText = new Text();
                foreach(XmlAttributeValue xmlAttributeValue in xmlAttribute.value)
                {
                    valueText.setText(xmlAttributeValue.language.ToString(), xmlAttributeValue.value);
                    commentText.setText(xmlAttributeValue.language.ToString(), xmlAttributeValue.comment);
                }

                if (quizData.verifyText(valueText, String.Format("element {0}, attribute type {1}", XmlElement.id, xmlAttributeType.id)) != 0) return -1;

                AttributeValue attributeValue = new AttributeValue(valueText, commentText, xmlAttributeType);
                
		        if (_attributeDictionary.ContainsKey(xmlAttributeType))
		        {
			        MessageLogger.addMessage(XmlLogLevelEnum.ERROR,
                        String.Format("Error in {0}: Attribute {1} is defined several times for element {2}",
                        quizData.DataFileName, xmlAttributeType.id, _xmlElement.id));
			        return -1;
		        }

                _attributeDictionary.Add(xmlAttributeType, attributeValue);

		        if (xmlAttributeType.canBeQuestion)
		        {
                    Text attributeKey = new Text();
                    foreach (string language in attributeValue.Value.LanguageList)
                    {
                        attributeKey.setText(language, String.Format("{0}+{1}+{2}", _type.id, xmlAttributeType.id, attributeValue.Value.getText(language)));
                    }
                    _attributeKeyList.Add(attributeKey);
		        }
	        }

            return 0;
        }

        internal int addNumericalAttributes(QuizData quizData)
        {
            foreach (XmlNumericalAttribute xmlNumericalAttribute in _xmlElement.numericalAttributeList)
	        {
                XmlNumericalAttributeType xmlNumericalAttributeType = quizData.getXmlNumericalAttributeType(xmlNumericalAttribute.type);
                NumericalAttributeValue numericalAttributeValue = new NumericalAttributeValue(xmlNumericalAttribute.value, xmlNumericalAttributeType);
                
                if (_numericalAttributeDictionary.ContainsKey(xmlNumericalAttributeType))
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR,
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
                    RelationType relationType = quizData.getRelationType(xmlRelation.type);

			        if (_type != relationType.StartType || linkedElement._type != relationType.EndType)
			        {
                        if (_type != relationType.StartType)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, 
                                String.Format("Error in {0}: Error in relation between element {1} and element {2}: The type of element {1} is {3}, instead of {4} expected by relation {5}",
                                quizData.DataFileName, _xmlElement.id, linkedElement.XmlElement.id, _type.id, relationType.StartType.id, xmlRelation.type));
                        }
                        else
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.ERROR,
                                String.Format("Error in {0}: Error in relation between element {1} and element {2}: The type of element {2} is {3}, instead of {4} expected by relation {5}",
                                quizData.DataFileName, _xmlElement.id, linkedElement.XmlElement.id, linkedElement._type.id, relationType.EndType.id, xmlRelation.type));
                        }

                        return -1;
			        }

			        if (relationType.Nature == RelationNatureEnum.RELATION_11)
			        {
                        if (addRelation1(quizData, linkedElement, relationType) != 0) return -1;
                        if (linkedElement.addRelation1(quizData, this, relationType.ReciprocalType) != 0) return -1;
			        }
			        else if (relationType.Nature == RelationNatureEnum.RELATION_1N)
			        {
                        if (addRelationN(linkedElement, relationType) != 0) return -1;
				        if (linkedElement.addRelation1 (quizData, this, relationType.ReciprocalType) != 0) return -1;
			        }
			        else
			        {
				        if (addRelationN (linkedElement, relationType) != 0) return -1;
				        if (linkedElement.addRelationN (this, relationType.ReciprocalType) != 0) return -1;
			        }
		        }
	        }

            return 0;
        }

        private int addRelation1(QuizData quizData, Element element, RelationType relationType)
        {
            if (_relation1Dictionary.ContainsKey(relationType))
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Error in {0}: Element {1} is linked to several elements ({2} and {3}) by relation {4}",
                    quizData.DataFileName, _xmlElement.id, _relation1Dictionary[relationType]._xmlElement.id, element._xmlElement.id, relationType.Id));
                return -1;
            }

            _relation1Dictionary.Add(relationType, element);
            return 0;
        }

        private int addRelationN(Element element, RelationType relationType)
        {
            if (!_relationNDictionary.ContainsKey(relationType)) _relationNDictionary.Add(relationType, new List<Element>());
            _relationNDictionary[relationType].Add(element);
            return 0;
        }
    }
}
