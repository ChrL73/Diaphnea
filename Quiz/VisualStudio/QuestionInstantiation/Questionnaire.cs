using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;

namespace QuestionInstantiation
{
    class Questionnaire
    {
        private QuizData _quizData;
        private Text _name = new Text();
        private List<Level> _levelList = new List<Level>();

        internal void instantiate(string path)
        {
            int result = 0;

            if (result == 0) result = loadData(path);
            if (result == 0) result = instantiateQuestions();
            if (result == 0) result = fillDatabase();
            if (result == 0) result = generateCode();

            if (result == 0) Console.WriteLine("Question instantitation terminated successfully for file {0}", path);
            else Console.WriteLine("Question instantitation terminated with errors for file {0}", path);

            Console.WriteLine();
        }

        private int loadData(string path)
        {
            XmlReader reader;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;

            try
            {
                reader = XmlReader.Create(path, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail to open file {0}", path);
                Console.WriteLine(e.Message);
                return -1;
            }

            Console.WriteLine("Reading configuration file " + path + " ...");

            XmlSerializer serializer = new XmlSerializer(typeof(XmlQuizData));
            XmlQuizData quizData = null;

            try
            {
                quizData = (XmlQuizData)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail to deserialize content of file {0}", path);
                Console.WriteLine(e.Message);
                if (e.InnerException != null) Console.WriteLine(e.InnerException.Message);
                return -1;
            }

            reader.Close();

            _quizData = new QuizData(path, quizData);

            MessageLogger.LogFileName = quizData.parameters.logFileName;
            MessageLogger.FileLogLevel = quizData.parameters.fileLogLevel;
            MessageLogger.ConsoleLogLevel = quizData.parameters.consoleLogLevel;

            return 0;
        }

        private int instantiateQuestions()
        {
            foreach (XmlName xmlName in _quizData.XmlQuizData.parameters.questionnaireName) _name.setText(xmlName.language.ToString(), xmlName.text);
            if (_quizData.verifyText(_name, String.Format("name of questionnaire {0}", _quizData.XmlQuizData.parameters.questionnaireId)) != 0) return -1;

            foreach (XmlLevel xmlLevel in _quizData.XmlQuizData.parameters.levelList)
            {
                Level level = new Level();
                if (level.initialize(_quizData, xmlLevel) != 0) return -1;
                _levelList.Add(level);
            }

            return 0;
        }

        private int fillDatabase()
        {
            if (_quizData.XmlQuizData.parameters.languageList.Where(x => x.status == XmlLanguageStatusEnum.TRANSLATION_COMPLETED).Count() == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Qestionnaire \"{0}\": No language with completed translation: Database not filled",
                    _quizData.XmlQuizData.parameters.questionnaireId));
                return -1;
            }

            Text.setCompletedTranslationDictionary(_quizData);

            MongoClient mongoClient = new MongoClient();
            IMongoDatabase database = mongoClient.GetDatabase(_quizData.XmlQuizData.parameters.databaseName);

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("questionnaire", _quizData.XmlQuizData.parameters.questionnaireId);
            IMongoCollection<BsonDocument> levelCollection = database.GetCollection<BsonDocument>("levels");
            levelCollection.DeleteMany(filter);
            IMongoCollection<BsonDocument> questionListsCollection = database.GetCollection<BsonDocument>("question_lists");
            questionListsCollection.DeleteMany(filter);
            IMongoCollection<BsonDocument> choiceListsCollection = database.GetCollection<BsonDocument>("choice_lists");
            choiceListsCollection.DeleteMany(filter);
            IMongoCollection<BsonDocument> questionnaireCollection = database.GetCollection<BsonDocument>("questionnaires");
            questionnaireCollection.DeleteMany(filter);

            Int32 totalQuestionCount = 0;
            foreach (Level level in _levelList) totalQuestionCount += level.QuestionCount;
            if (totalQuestionCount == 0) return 0;
            
            BsonDocument questionnaireDocument = new BsonDocument()
            {
                { "questionnaire", _quizData.XmlQuizData.parameters.questionnaireId },
                { "map_id", _quizData.XmlQuizData.parameters.mapId },
                { "name", _name.getBsonDocument() }
            };

            BsonArray languagesArray = new BsonArray();
            foreach (XmlLanguage language in _quizData.XmlQuizData.parameters.languageList)
            {
                if (language.status == XmlLanguageStatusEnum.TRANSLATION_COMPLETED)
                {
                    BsonDocument languageDocument = new BsonDocument()
                    {
                        { "id", language.id.ToString() },
                        { "name", language.name }
                    };
                    languagesArray.Add(languageDocument);
                }
            }
            BsonDocument languagesDocument = new BsonDocument()
            {
                { "languages", languagesArray }
            };
            questionnaireDocument.AddRange(languagesDocument);

            questionnaireCollection.InsertOne(questionnaireDocument);

            foreach (Level level in _levelList)
            {
                if (level.QuestionCount > 0)
                {
                    if (level.fillDataBase(database) != 0) return -1;
                }
            }

            return 0;
        }

        private int generateCode()
        {
            if (!Directory.Exists(_quizData.XmlQuizData.parameters.generationDir))
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Generation folder does not exist ({0})", _quizData.XmlQuizData.parameters.generationDir));
                return -1;
            }

            foreach (Level level in _levelList)
            {
                if (level.QuestionCount > 0)
                {
                    if (level.generateCode() != 0) return -1;
                }
            }

            return 0;
        }
    }
}
