using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace QuestionInstantiation
{
    class AttributeOrderCategory : Category
    {
        private readonly SortedDictionary<double, List<Element>> _choiceDictionary = new SortedDictionary<double, List<Element>>();
        private readonly List<AttributeOrderElement> _elementList = new List<AttributeOrderElement>();
        private readonly XmlNumericalAttributeType _numericalAttributeType;
        private readonly Text _questionText;
        private readonly XmlAttributeOrderModeEnum _mode;
        private readonly double _distribParameterCorrection;
        private readonly string _valueFormat;
        private int _maxIndex;

        internal AttributeOrderCategory (int weightIndex, string questionNameInLog, QuizData quizData, XmlNumericalAttributeType numericalAttributeType,
                                         Text questionText, XmlAttributeOrderModeEnum mode, double distribParameterCorrection, MapParameters mapParameters)
            : base(weightIndex, questionNameInLog, quizData, mapParameters)
        {
            _numericalAttributeType = numericalAttributeType;
            _questionText = questionText;
            _mode = mode;
            _distribParameterCorrection = distribParameterCorrection;
            _maxIndex = -1;

            _valueFormat = "0.";
            int i, n = Int32.Parse(_numericalAttributeType.displayPrecision);
            for (i = 0; i < n; ++i) _valueFormat += "#";
        }

        internal int ChoiceCount { get { return _choiceDictionary.Count; } }

        internal void addAnswer (double value, Element element)
        {
            if (_mode == XmlAttributeOrderModeEnum.GREATEST) value = -value;
            if (!_choiceDictionary.ContainsKey(value)) _choiceDictionary.Add(value, new List<Element>());
            _choiceDictionary[value].Add(element);
        }

        internal int setElementVector(int choiceCount)
        {
            foreach (KeyValuePair<double, List<Element>> pair in _choiceDictionary)
            {
                foreach (Element answerElement in pair.Value)
                {
                    AttributeOrderElement element = new AttributeOrderElement(pair.Key, answerElement);
                    _elementList.Add(element);
                }
            }

            int i, n = _elementList.Count;
            int minIndex = n - 1;
            bool minIndexExists = false;
            for (i = n - 2; i >= 0; --i)
            {
                int j;
                for (j = i + 1; j <= minIndex; j++)
                {
                    double d = _elementList[i].AttributeValue - _elementList[j].AttributeValue;
                    if (d < 0) d = -d;
                    if (d > _numericalAttributeType.ambiguityThreshold)
                    {
                        minIndex = j;
                        minIndexExists = true;
                        break;
                    }
                }
                if (minIndexExists)
                {
                    _elementList[i].MinAnswerIndex = minIndex;
                    if (minIndex + choiceCount <= n + 1) ++_maxIndex;
                }
            }

            if (_maxIndex < 0) return -1;
            return 0;
        }

        internal override BsonDocument getBsonDocument(IMongoDatabase database, string questionnaireId)
        {
            IMongoCollection<BsonDocument> choiceListsCollection = database.GetCollection<BsonDocument>("choice_lists");
            BsonDocument choiceListDocument = getChoiceListDocument(questionnaireId);
            choiceListsCollection.InsertOne(choiceListDocument);

            BsonDocument categoryDocument = new BsonDocument()
            {
                { "type", "AttributeOrder" },
                { "question", _questionText.getBsonDocument() },
                { "choice_count", _elementList.Count },
                { "choice_list", choiceListDocument.GetValue("_id") },
                { "weight_index", WeightIndex },
                { "distrib_parameter_correction", _distribParameterCorrection },
                { "max_index", _maxIndex },
                { "map_parameters", getMapParameterBsonDocument() }
            };

            return categoryDocument;
        }

        private BsonDocument getChoiceListDocument(string questionnaireId)
        {
            BsonArray choicesArray = new BsonArray();
            foreach (AttributeOrderElement element in _elementList)
            {
                double value = _mode == XmlAttributeOrderModeEnum.LOWEST ? element.AttributeValue : -element.AttributeValue;
                string comment = String.Format("{0}{1}", value.ToString(_valueFormat, CultureInfo.CreateSpecificCulture("en-US")), _numericalAttributeType.unit);

                BsonDocument choiceDocument = new BsonDocument()
                {
                    { "choice", element.Element.Name.getBsonDocument() },
                    { "comment", comment },
                    { "min_index", element.MinAnswerIndex },
                    { "map_id", element.Element.XmlElement.mapId == null ? "" : element.Element.XmlElement.mapId.Substring(2) }
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

        internal override int generateCode(List<CodeGenerator> codeGeneratorList)
        {
            return 0;
        }
    }
}
