using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    abstract class Category
    {
        private readonly Int32 _weightIndex;
        private readonly string _questionNameInLog;
        private readonly QuizData _quizData;
        private readonly MapParameters _mapParameters;

        protected Category(int weightIndex, string questionNameInLog, QuizData quizData, MapParameters mapParameters)
        {
            _weightIndex = weightIndex;
            _questionNameInLog = questionNameInLog;
            _quizData = quizData;
            _mapParameters = mapParameters;
        }

        protected Int32 WeightIndex
        {
            get { return _weightIndex; }
        }

        protected string QuestionNameInLog
        {
            get { return _questionNameInLog; }
        }

        protected QuizData QuizData
        {
            get { return _quizData; }
        }

        protected BsonDocument getMapParameterBsonDocument()
        {
            if (_mapParameters == null) throw new NotImplementedException();

            BsonDocument mapParametersDocument = new BsonDocument()
            {
                { "framing_level", _mapParameters.FramingLevel },
                { "question", getMapSubParametersBsonDocument(_mapParameters.QuestionParameters) },
                { "answer", getMapSubParametersBsonDocument(_mapParameters.AnswerParameters) },
                { "wrong_choice", getMapSubParametersBsonDocument(_mapParameters.WrongChoiceParameters) }
            };

            if (_mapParameters.AnswerSelectionMode != null)
            {
                mapParametersDocument.AddRange(new BsonDocument() { { "answer_selection_mode", _mapParameters.AnswerSelectionMode.ToString() } });
            }

            return mapParametersDocument;
        }

        private BsonDocument getMapSubParametersBsonDocument(XmlMapSubParameters subParameters)
        {
            BsonArray categoryArray = new BsonArray();
            int drawDepth = 0;
            int framingMode = 0;
            string selectionMode = "";

            if (subParameters != null)
            {
                drawDepth = Int32.Parse(subParameters.drawDepth);
                selectionMode = subParameters.categorySelectionMode.ToString();
                if (subParameters.framingMode == XmlFramingModeEnum.ONLY_MAIN_ELEMENT) framingMode = 1;
                else if (subParameters.framingMode == XmlFramingModeEnum.ALL_LINKED_ELEMENTS) framingMode = 2;

                if (subParameters.category != null)
                {
                    foreach (XmlMapCategory category in subParameters.category)
                    {
                        categoryArray.Add(Int32.Parse(category.categoryIndexInMapConfigFile));
                    }
                }
            }

            BsonDocument document = new BsonDocument()
            {
                { "draw_depth", drawDepth },
                { "category_selection_mode", selectionMode },
                { "categories", categoryArray },
                { "framing_mode", framingMode}
            };

            return document;
        }

        abstract internal BsonDocument getBsonDocument(IMongoDatabase database, string questionnaireId);
    }
}
