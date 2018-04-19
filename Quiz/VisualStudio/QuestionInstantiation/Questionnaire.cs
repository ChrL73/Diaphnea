﻿using MongoDB.Bson;
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
using Schemas;

namespace QuestionInstantiation
{
    class Questionnaire
    {
        private QuizData _quizData;
        private Text _name = new Text();
        private List<Level> _levelList = new List<Level>();

        internal int instantiate(string path)
        {
            int result = 0;

            if (result == 0) result = loadData(path);
            if (result == 0) result = instantiateQuestions();
            if (result == 0) result = fillDatabase();
            if (result == 0) result = generateCode();

            if (result == 0) Console.WriteLine("Question instantitation terminated successfully for file {0}", path);
            else Console.WriteLine("Question instantitation terminated with errors for file {0}", path);

            Console.WriteLine();

            return result;
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

            bool testMode = false;
            foreach (XmlAttributeQuestionCategory category in _quizData.XmlQuizData.questionCategories.attributeQuestionCategoryList)
            {
                if (category.test)
                {
                    testMode = true;
                    break;
                }
            }

            if (!testMode)
            {
                foreach (XmlRelation1QuestionCategory category in _quizData.XmlQuizData.questionCategories.relation1QuestionCategoryList)
                {
                    if (category.test)
                    {
                        testMode = true;
                        break;
                    }
                }
            }

            if (!testMode)
            {
                foreach (XmlRelationNQuestionCategory category in _quizData.XmlQuizData.questionCategories.relationNQuestionCategoryList)
                {
                    if (category.test)
                    {
                        testMode = true;
                        break;
                    }
                }
            }

            if (!testMode)
            {
                foreach (XmlAttributeOrderQuestionCategory category in _quizData.XmlQuizData.questionCategories.attributeOrderQuestionCategoryList)
                {
                    if (category.test)
                    {
                        testMode = true;
                        break;
                    }
                }
            }

            if (!testMode)
            {
                foreach (XmlRelationLimitQuestionCategory category in _quizData.XmlQuizData.questionCategories.relationLimitQuestionCategoryList)
                {
                    if (category.test)
                    {
                        testMode = true;
                        break;
                    }
                }
            }

            if (!testMode)
            {
                foreach (XmlRelationOrderQuestionCategory category in _quizData.XmlQuizData.questionCategories.relationOrderQuestionCategoryList)
                {
                    if (category.test)
                    {
                        testMode = true;
                        break;
                    }
                }
            }

            if (!testMode)
            {
                foreach (XmlRelationExistenceQuestionCategory category in _quizData.XmlQuizData.questionCategories.relationExistenceQuestionCategoryList)
                {
                    if (category.test)
                    {
                        testMode = true;
                        break;
                    }
                }
            }

            foreach (XmlLevel xmlLevel in _quizData.XmlQuizData.parameters.levelList)
            {
                Level level = new Level();
                if (level.initialize(_quizData, xmlLevel, testMode) != 0) return -1;
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

            // Comment database filling while testing the code generation

            /*MongoClient mongoClient = new MongoClient();
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
            }*/

            return 0;
        }

        private int generateCode()
        {
            if (!Directory.Exists(_quizData.XmlQuizData.parameters.cppGenerationDir))
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Generation folder does not exist ({0})", _quizData.XmlQuizData.parameters.cppGenerationDir));
                return -1;
            }

            foreach (Level level in _levelList)
            {
                if (level.QuestionCount > 0)
                {
                    if (level.generateCode() != 0) return -1;
                }
            }

            MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("JavaScript code generation..."));

            List<string> generatedSection = new List<string>();

            generatedSection.Add("   {");
            generatedSection.Add(String.Format("      id: '{0}',", _quizData.XmlQuizData.parameters.questionnaireId));
            generatedSection.Add(String.Format("      mapId: '{0}',", _quizData.XmlQuizData.parameters.mapId));

            List<string> names = new List<string>();
            foreach (XmlName name in _quizData.XmlQuizData.parameters.questionnaireName)
            {
                names.Add(String.Format(" {0}: '{1}'", name.language, name.text));
            }
            generatedSection.Add(String.Format("      name: {{{0} }},", String.Join(",", names)));

            generatedSection.Add("      languages:");
            generatedSection.Add("      [");
            foreach (XmlLanguage language in _quizData.XmlQuizData.parameters.languageList)
            {
                generatedSection.Add(String.Format("         {{ id: '{0}', name: '{1}' }},", language.id, language.name));
            }
            generatedSection.Add("      ],");

            generatedSection.Add("      levels:");
            generatedSection.Add("      [");
            foreach (XmlLevel level in _quizData.XmlQuizData.parameters.levelList)
            {
                names.Clear();
                foreach (XmlName name in level.name)
                {
                    names.Add(String.Format(" {0}: '{1}'", name.language, name.text));
                }
                generatedSection.Add(String.Format("         {{ id: '{0}', name: {{{1} }} }},",
                                     level.levelId, String.Join(",", names)));
            }
            generatedSection.Add("      ],");
            generatedSection.Add("   },");

            string jsPath = String.Format("{0}/generated_data.js", _quizData.XmlQuizData.parameters.jsGenerationDir);
            string[] lines = null;

            try
            {
                lines = File.ReadAllLines(jsPath);
            }
            catch (Exception)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Failed to read file ({0})", jsPath));
                return -1;
            }
            List<List<string>> jsFileContent = new List<List<string>>();

            List<string> currentSection = null;
            string currentQuestionnaireId = null;
            bool sectionToReplaceReached = false;
            bool generatedSectionAdded = false;

            foreach (string line in lines)
            {
                if (currentSection == null && line.Contains("{"))
                {
                    currentSection = new List<string>();
                }

                if (currentSection != null)
                {
                    if (currentQuestionnaireId == null)
                    {
                        int index = line.IndexOf("'");
                        if (index != -1)
                        {
                            string s = line.Substring(index + 1);
                            index = s.IndexOf("'");
                            if (index != -1)
                            {
                                currentQuestionnaireId = s.Substring(0, index);
                                if (currentQuestionnaireId == _quizData.XmlQuizData.parameters.questionnaireId)
                                {
                                    sectionToReplaceReached = true;
                                }
                            }
                        }
                    }

                    currentSection.Add(line);
                }

                if (currentSection != null && line.Contains("}") && !line.Contains("{"))
                {
                    if (sectionToReplaceReached && !generatedSectionAdded)
                    {
                        jsFileContent.Add(generatedSection);
                        generatedSectionAdded = true;
                    }
                    else
                    {
                        jsFileContent.Add(currentSection);
                    }

                    currentSection = null;
                    currentQuestionnaireId = null;
                }
            }

            if (!generatedSectionAdded)
            {
                jsFileContent.Add(generatedSection);
            }

            using (TextWriter tw = new StreamWriter(jsPath))
            {
                tw.WriteLine("// This file is automatically updated by 'QuestionInstantiation.exe'");
                tw.WriteLine("// Manually editing this file may cause this automatic update to fail");
                tw.WriteLine("// (In particular, do not add or remove 'newline' characters)");
                tw.WriteLine("module.exports =");
                tw.WriteLine("[");

                foreach (List<string> section in jsFileContent)
                {
                    foreach (string line in section) tw.WriteLine(line);
                }

                tw.WriteLine("];");
            }

            return 0;
        }
    }
}
