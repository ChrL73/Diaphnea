using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class QuizData
    {
        private readonly XmlQuizData _xmlQuizData;
        private readonly Dictionary<string, XmlElementType> _elementTypeDictionary = new Dictionary<string, XmlElementType>();
        private readonly Dictionary<string, XmlAttributeType> _attributeTypeDictionary = new Dictionary<string, XmlAttributeType>();
        private readonly Dictionary<string, XmlNumericalAttributeType> _numericalAttributeTypeDictionary = new Dictionary<string, XmlNumericalAttributeType>();
        private readonly Dictionary<string, XmlRelation11Type> _relation11TypeDictionary = new Dictionary<string, XmlRelation11Type>();
        private readonly Dictionary<string, XmlRelation1NType> _relation1NTypeDictionary = new Dictionary<string, XmlRelation1NType>();
        private readonly Dictionary<string, XmlRelationNNType> _relationNNTypeDictionary = new Dictionary<string, XmlRelationNNType>();
        private readonly Dictionary<string, XmlElement> _elementDictionary = new Dictionary<string, XmlElement>();

        internal QuizData(XmlQuizData xmlQuizData)
        {
            _xmlQuizData = xmlQuizData;
            foreach (XmlElementType elementType in _xmlQuizData.typeDefinitions.elementTypeList) _elementTypeDictionary.Add(elementType.id, elementType);
            foreach (XmlAttributeType attributeType in _xmlQuizData.typeDefinitions.attributeTypeList) _attributeTypeDictionary.Add(attributeType.id, attributeType);
            foreach (XmlNumericalAttributeType numericalAttributeType in _xmlQuizData.typeDefinitions.numericalAttributeTypeList) _numericalAttributeTypeDictionary.Add(numericalAttributeType.id, numericalAttributeType);
            foreach (XmlRelation11Type relation11Type in _xmlQuizData.typeDefinitions.relation11TypeList) _relation11TypeDictionary.Add(relation11Type.id, relation11Type);
            foreach (XmlRelation1NType relation1NType in _xmlQuizData.typeDefinitions.relation1NTypeList) _relation1NTypeDictionary.Add(relation1NType.id, relation1NType);
            foreach (XmlRelationNNType relationNNType in _xmlQuizData.typeDefinitions.relationNNTypeList) _relationNNTypeDictionary.Add(relationNNType.id, relationNNType);
            foreach (XmlElement element in _xmlQuizData.elementList) _elementDictionary.Add(element.id, element);
        }

        internal XmlQuizData XmlQuizData
        {
            get { return _xmlQuizData; }
        }

        internal XmlElementType getElementType(string id)
        {
            XmlElementType elementType;
            if (_elementTypeDictionary.TryGetValue(id, out elementType)) return elementType;
            return null;
        }

        internal XmlAttributeType getAttributeType(string id)
        {
            XmlAttributeType attributeType;
            if (_attributeTypeDictionary.TryGetValue(id, out attributeType)) return attributeType;
            return null;
        }

        internal XmlNumericalAttributeType getNumericalAttributeType(string id)
        {
            XmlNumericalAttributeType numericalAttributeType;
            if (_numericalAttributeTypeDictionary.TryGetValue(id, out numericalAttributeType)) return numericalAttributeType;
            return null;
        }

        internal XmlRelation11Type getRelation11Type(string id)
        {
            XmlRelation11Type relation11Type;
            if (_relation11TypeDictionary.TryGetValue(id, out relation11Type)) return relation11Type;
            return null;
        }

        internal XmlRelation1NType getRelation1NType(string id)
        {
            XmlRelation1NType relation1NType;
            if (_relation1NTypeDictionary.TryGetValue(id, out relation1NType)) return relation1NType;
            return null;
        }

        internal XmlRelationNNType getRelationNNType(string id)
        {
            XmlRelationNNType relationNNType;
            if (_relationNNTypeDictionary.TryGetValue(id, out relationNNType)) return relationNNType;
            return null;
        }

        internal XmlElement getElement(string id)
        {
            XmlElement element;
            if (_elementDictionary.TryGetValue(id, out element)) return element;
            return null;
        }
    }
}
