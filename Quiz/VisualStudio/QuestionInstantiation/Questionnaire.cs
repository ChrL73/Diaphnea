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

namespace QuestionInstantiation
{
    class Questionnaire
    {
        private QuizData _quizData;
        private List<Level> _levelList = new List<Level>();

        internal void instantiate(string path)
        {
            int result = 0;

            if (result == 0) result = loadData(path);
            if (result == 0) result = instantiateQuestions();
            if (result == 0) result = fillDatabase();

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

        int instantiateQuestions()
        {
            foreach (XmlLevel xmlLevel in _quizData.XmlQuizData.parameters.levelList)
            {
                Level level = new Level();
                if (level.initialize(_quizData, xmlLevel) != 0) return -1;
                _levelList.Add(level);
            }

            return 0;
        }

        int fillDatabase()
        {
            MongoClient mongoClient = new MongoClient();
            IMongoDatabase database = mongoClient.GetDatabase(_quizData.XmlQuizData.parameters.databaseName);

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("questionnaire", _quizData.XmlQuizData.parameters.questionnaireId);
            IMongoCollection<BsonDocument> levelCollection = database.GetCollection<BsonDocument>("levels");
            levelCollection.DeleteMany(filter);
            IMongoCollection<BsonDocument> questionListsCollection = database.GetCollection<BsonDocument>("question_lists");
            questionListsCollection.DeleteMany(filter);
            IMongoCollection<BsonDocument> questionnaireCollection = database.GetCollection<BsonDocument>("questionnaires");
            questionnaireCollection.DeleteMany(filter);

            Int32 totalQuestionCount = 0;
            foreach (Level level in _levelList) totalQuestionCount += level.QuestionCount;
            if (totalQuestionCount == 0) return 0;
            
            BsonDocument questionnaireDocument = new BsonDocument()
            {
                { "questionnaire", _quizData.XmlQuizData.parameters.questionnaireId }
            };

            BsonArray languagesArray = new BsonArray();
            foreach (XmlLanguage language in _quizData.XmlQuizData.parameters.languageList)
            {
                BsonDocument languageDocument = new BsonDocument()
                {
                    { "id", language.id },
                    { "name", language.name }
                };
                languagesArray.Add(languageDocument);
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
    }
}
