using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace QuestionInstantiation
{
    class QuizData
    {
        private readonly string _dataFileName;
        private readonly XmlQuizData _xmlQuizData;
        private readonly Dictionary<string, XmlLanguage> _languageDictionary = new Dictionary<string, XmlLanguage>();
        private readonly Dictionary<string, XmlElementType> _elementTypeDictionary = new Dictionary<string, XmlElementType>();
        private readonly Dictionary<string, XmlAttributeType> _attributeTypeDictionary = new Dictionary<string, XmlAttributeType>();
        private readonly Dictionary<string, XmlNumericalAttributeType> _numericalAttributeTypeDictionary = new Dictionary<string, XmlNumericalAttributeType>();
        private readonly Dictionary<string, XmlRelation11Type> _relation11TypeDictionary = new Dictionary<string, XmlRelation11Type>();
        private readonly Dictionary<string, XmlRelation1NType> _relation1NTypeDictionary = new Dictionary<string, XmlRelation1NType>();
        private readonly Dictionary<string, XmlRelationNNType> _relationNNTypeDictionary = new Dictionary<string, XmlRelationNNType>();
        private readonly Dictionary<string, XmlElement> _elementDictionary = new Dictionary<string, XmlElement>();
        private readonly Dictionary<string, RelationType> _relationTypeDictionary = new Dictionary<string, RelationType>();

        internal QuizData(string dataFileName, XmlQuizData xmlQuizData)
        {
            _dataFileName = dataFileName;
            _xmlQuizData = xmlQuizData;
            foreach (XmlLanguage language in _xmlQuizData.parameters.languageList) _languageDictionary.Add(language.id.ToString(), language);
            foreach (XmlElementType elementType in _xmlQuizData.typeDefinitions.elementTypeList) _elementTypeDictionary.Add(elementType.id, elementType);
            foreach (XmlAttributeType attributeType in _xmlQuizData.typeDefinitions.attributeTypeList) _attributeTypeDictionary.Add(attributeType.id, attributeType);
            foreach (XmlNumericalAttributeType numericalAttributeType in _xmlQuizData.typeDefinitions.numericalAttributeTypeList) _numericalAttributeTypeDictionary.Add(numericalAttributeType.id, numericalAttributeType);
            foreach (XmlRelation11Type relation11Type in _xmlQuizData.typeDefinitions.relation11TypeList) _relation11TypeDictionary.Add(relation11Type.id, relation11Type);
            foreach (XmlRelation1NType relation1NType in _xmlQuizData.typeDefinitions.relation1NTypeList) _relation1NTypeDictionary.Add(relation1NType.id, relation1NType);
            foreach (XmlRelationNNType relationNNType in _xmlQuizData.typeDefinitions.relationNNTypeList) _relationNNTypeDictionary.Add(relationNNType.id, relationNNType);
            foreach (XmlElement element in _xmlQuizData.elementList) _elementDictionary.Add(element.id, element);

            BuildRelationTypes();
            verifyGeographicalNumericalAttributes();
        }

        private void BuildRelationTypes()
        {
            foreach (XmlRelation11Type xmlRelation11Type in _xmlQuizData.typeDefinitions.relation11TypeList)
            {
                RelationType directType = new RelationType(xmlRelation11Type.id,  RelationNatureEnum.RELATION_11, getXmlElementType(xmlRelation11Type.startType),
                                                           getXmlElementType(xmlRelation11Type.endType), RelationWayEnum.DIRECT,  XmlCheckSymetryEnum.NO);

                RelationType inverseType = new RelationType(xmlRelation11Type.id, RelationNatureEnum.RELATION_11, getXmlElementType(xmlRelation11Type.endType),
                                                           getXmlElementType(xmlRelation11Type.startType), RelationWayEnum.INVERSE, XmlCheckSymetryEnum.NO);

                directType.ReciprocalType = inverseType;
                inverseType.ReciprocalType = directType;
                _relationTypeDictionary.Add(xmlRelation11Type.id, directType);
            }

            foreach (XmlRelation1NType xmlRelation1NType in _xmlQuizData.typeDefinitions.relation1NTypeList)
            {
                RelationType directType = new RelationType(xmlRelation1NType.id, RelationNatureEnum.RELATION_1N, getXmlElementType(xmlRelation1NType.startType),
                                                           getXmlElementType(xmlRelation1NType.endType), RelationWayEnum.DIRECT, XmlCheckSymetryEnum.NO);

                RelationType inverseType = new RelationType(xmlRelation1NType.id, RelationNatureEnum.RELATION_1N, getXmlElementType(xmlRelation1NType.endType),
                                                           getXmlElementType(xmlRelation1NType.startType), RelationWayEnum.INVERSE, XmlCheckSymetryEnum.NO);

                directType.ReciprocalType = inverseType;
                inverseType.ReciprocalType = directType;
                _relationTypeDictionary.Add(xmlRelation1NType.id, directType);
            }

            foreach (XmlRelationNNType xmlRelationNNType in _xmlQuizData.typeDefinitions.relationNNTypeList)
            {
                XmlCheckSymetryEnum checkSymetry = XmlCheckSymetryEnum.NO;
                if (xmlRelationNNType.checkSymetrySpecified) checkSymetry = xmlRelationNNType.checkSymetry;

                if (xmlRelationNNType.startType != xmlRelationNNType.endType && checkSymetry != XmlCheckSymetryEnum.NO)
                {
                    checkSymetry = XmlCheckSymetryEnum.NO;
                    MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                        "Relation {0}: Impossible to check symetry, because start type ({1}) is different from end type ({2}). Value {3} of checkSymetry attribute is ignored",
                        xmlRelationNNType.id, xmlRelationNNType.startType, xmlRelationNNType.endType, xmlRelationNNType.checkSymetry));
                }

                RelationType directType = new RelationType(xmlRelationNNType.id, RelationNatureEnum.RELATION_NN, getXmlElementType(xmlRelationNNType.startType),
                                                           getXmlElementType(xmlRelationNNType.endType), RelationWayEnum.DIRECT, checkSymetry);

                RelationType inverseType = new RelationType(xmlRelationNNType.id, RelationNatureEnum.RELATION_NN, getXmlElementType(xmlRelationNNType.endType),
                                                           getXmlElementType(xmlRelationNNType.startType), RelationWayEnum.INVERSE, checkSymetry);

                directType.ReciprocalType = inverseType;
                inverseType.ReciprocalType = directType;
                _relationTypeDictionary.Add(xmlRelationNNType.id, directType);
            }
        }

        private void verifyGeographicalNumericalAttributes()
        {
            XmlNumericalAttributeType latitudeAttributeType = null;
            XmlNumericalAttributeType longitudeAttributeType = null;
            foreach (XmlNumericalAttributeType xmlNumericalAttributeType in _xmlQuizData.typeDefinitions.numericalAttributeTypeList)
            {
                if (xmlNumericalAttributeType.id == "nt_Latitude") latitudeAttributeType = xmlNumericalAttributeType;
                else if (xmlNumericalAttributeType.id == "nt_Longitude") longitudeAttributeType = xmlNumericalAttributeType;
            }

            if (latitudeAttributeType == null)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                    "File {0} contains no numerical attribute type called \"nt_Latitude\". Value \"elementLocation\" of parameter \"answerProximityCriterion\" will not be taken into account",
                    _dataFileName));
            }
            
            if (longitudeAttributeType == null)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                    "File {0} contains no numerical attribute type called \"nt_Longitude\". Value \"elementLocation\" of parameter \"answerProximityCriterion\" can not be used",
                    _dataFileName));
            }
            
            if (latitudeAttributeType != null && latitudeAttributeType.unit != "°")
            {
                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                    "File {0}: Unit of numerical attribute type \"nt_Latitude\" is \"{1}\" instead of \"°\" expected. If latitudes are not in degrees, questions with \"elementLocation=answerProximityCriterion\" can not be used",
                    _dataFileName, latitudeAttributeType.unit));
            }

            if (longitudeAttributeType != null && longitudeAttributeType.unit != "°")
            {
                MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                    "File {0}: Unit of numerical attribute type \"nt_Longitude\" is \"{1}\" instead of \"°\" expected. If longitudes are not in degrees, questions with \"elementLocation=answerProximityCriterion\" will not be build as expected",
                    _dataFileName, longitudeAttributeType.unit));
            }
        }

        internal XmlQuizData XmlQuizData
        {
            get { return _xmlQuizData; }
        }

        internal string DataFileName
        {
            get { return _dataFileName; }
        }

        internal XmlLanguage getXmlLanguage(string id)
        {
            XmlLanguage language;
            if (_languageDictionary.TryGetValue(id, out language)) return language;
            return null;
        }

        internal int verifyText(Text text, string description)
        {
            foreach (XmlLanguage language in _xmlQuizData.parameters.languageList)
            {
                if (text.getText(language.id.ToString()) == null)
                {
                    if (language.status == XmlLanguageStatusEnum.TRANSLATION_COMPLETED)
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Language {0}, translation missing for: {1}", language.id, description));
                        return -1;
                    }
                    else if (language.status == XmlLanguageStatusEnum.TRANSLATION_IN_PROGRESS)
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Language {0}, translation missing for: {1}", language.id, description));
                    }
                }
            }

            return 0;
        }

        internal XmlElementType getXmlElementType(string id)
        {
            XmlElementType elementType;
            if (_elementTypeDictionary.TryGetValue(id, out elementType)) return elementType;
            return null;
        }

        internal XmlAttributeType getXmlAttributeType(string id)
        {
            XmlAttributeType attributeType;
            if (_attributeTypeDictionary.TryGetValue(id, out attributeType)) return attributeType;
            return null;
        }

        internal XmlNumericalAttributeType getXmlNumericalAttributeType(string id)
        {
            XmlNumericalAttributeType numericalAttributeType;
            if (_numericalAttributeTypeDictionary.TryGetValue(id, out numericalAttributeType)) return numericalAttributeType;
            return null;
        }

        internal XmlNumericalAttributeType getLongitudeNumericalAttributeType()
        {
            return getXmlNumericalAttributeType("nt_Longitude");
        }

        internal XmlNumericalAttributeType getLatitudeNumericalAttributeType()
        {
            return getXmlNumericalAttributeType("nt_Latitude");
        }

        internal XmlRelation11Type getXmlRelation11Type(string id)
        {
            XmlRelation11Type relation11Type;
            if (_relation11TypeDictionary.TryGetValue(id, out relation11Type)) return relation11Type;
            return null;
        }

        internal XmlRelation1NType getXmlRelation1NType(string id)
        {
            XmlRelation1NType relation1NType;
            if (_relation1NTypeDictionary.TryGetValue(id, out relation1NType)) return relation1NType;
            return null;
        }

        internal XmlRelationNNType getXmlRelationNNType(string id)
        {
            XmlRelationNNType relationNNType;
            if (_relationNNTypeDictionary.TryGetValue(id, out relationNNType)) return relationNNType;
            return null;
        }

        internal XmlElement getXmlElement(string id)
        {
            XmlElement element;
            if (_elementDictionary.TryGetValue(id, out element)) return element;
            return null;
        }

        internal RelationType getRelationType(string id)
        {
            RelationType relationType;
            if (_relationTypeDictionary.TryGetValue(id, out relationType)) return relationType;
            return null;
        }

        internal Dictionary<string, RelationType> RelationTypeDictionary
        {
            get { return _relationTypeDictionary; }
        }
    }
}
