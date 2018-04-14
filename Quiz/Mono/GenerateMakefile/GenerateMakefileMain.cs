using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Schemas;

namespace GenerateMakefile
{
    class GenerateMakefileMain
    {
        private static List<Rule> _ruleList = new List<Rule>();

        private static void Main(string[] args)
        {
            int result = 0;

            if (result == 0) result = addCppFileGeneration();

            if (result == 0)
            {
                addQuestionInstantiationBuild();
                addXsdGeneration();

                generate();
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static int addCppFileGeneration()
        {
            List<string> cppFileNames = new List<string>()
            {
                "AttributeOrderCategories",
                "Choices",
                "DoubleArrays",
                "IntArrays",
                "Level_",
                "MapParameterss",
                "MapSubParameterss",
                "MultipleAnswerCategories",
                "MultipleAnswerQuestions",
                "RelationOrderCategories",
                "RelationOrderQuestions",
                "SimpleAnswerCategories",
                "SimpleAnswerQuestions",
                "Strings"
            };

            string[] files = Directory.GetFiles("../../../../VisualStudio/QuestionInstantiation", "QuizData*.xml");
            foreach (string file in files)
            {
                XmlReader reader;
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;

                try
                {
                    reader = XmlReader.Create(file, settings);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Fail to open file {0}", file);
                    Console.WriteLine(e.Message);
                    return -1;
                }

                Console.WriteLine("Reading configuration file " + file + " ...");

                XmlSerializer serializer = new XmlSerializer(typeof(XmlQuizData));

                XmlQuizData quizData;
                try
                {
                    quizData = (XmlQuizData)serializer.Deserialize(reader);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Fail to deserialize content of file {0}", file);
                    Console.WriteLine(e.Message);
                    if (e.InnerException != null) Console.WriteLine(e.InnerException.Message);
                    return -1;
                }

                reader.Close();

                foreach (XmlLanguage language in quizData.parameters.languageList)
                {
                    if (language.status == XmlLanguageStatusEnum.TRANSLATION_COMPLETED)
                    {
                        foreach (XmlLevel level in quizData.parameters.levelList)
                        {
                            foreach (string fileName in cppFileNames)
                            {
                                Rule rule = new Rule();
                                rule.Target = String.Format("NodeJS/produce_questions2/generated_code/{0}{1}{2}/{3}.cpp",
                                                            quizData.parameters.questionnaireId, level.levelId, language.id, fileName);
                                rule.Dependencies.Add("Mono/QuestionInstantiation/bin/Debug/QuestionInstantiation.exe");
                                rule.Dependencies.Add(String.Format("VisualStudio/QuestionInstantiation/{0}", Path.GetFileName(file)));
                                rule.Commands.Add(String.Format(
                                    "cd Mono/QuestionInstantiation/bin/Debug && mono QuestionInstantiation.exe ../../../../VisualStudio/QuestionInstantiation/{0}",
                                    Path.GetFileName(file)));
                                _ruleList.Add(rule);
                            }
                        }
                    }
                }
            }

            return 0;
        }

        private static void addQuestionInstantiationBuild()
        {
            Rule rule = new Rule();
            rule.Target = "Mono/QuestionInstantiation/bin/Debug/QuestionInstantiation.exe";
            rule.Commands.Add("cd Mono/QuestionInstantiation && mdtool build QuestionInstantiation.csproj");
            rule.Dependencies.Add("Mono/QuestionInstantiation/QuestionInstantiation.csproj");

            string[] files = Directory.GetFiles("../../../../VisualStudio/QuestionInstantiation", "*.cs");
            foreach (string file in files)
            {
                rule.Dependencies.Add(file.Substring(12));
            }

            _ruleList.Add(rule);
        }

        private static void addXsdGeneration()
        {
            Rule rule = new Rule();
            rule.Target = "VisualStudio/QuestionInstantiation/XmlQuizData.cs";
            rule.Dependencies.Add("VisualStudio/QuestionInstantiation/XmlQuizData.xsd");
            rule.Commands.Add("cd VisualStudio/QuestionInstantiation && xsd XmlQuizData.xsd /classes");
            _ruleList.Add(rule);
        }

        private static void generate()
        {
            using (StreamWriter file = new StreamWriter("../../../../Makefile", false))
            {
                foreach (Rule rule in _ruleList)
                {
                    file.Write(String.Format("{0}: ", rule.Target));
                    file.WriteLine(String.Join(" ", rule.Dependencies));
                    foreach (string command in rule.Commands) file.WriteLine("\t{0}", command);
                    file.WriteLine();
                }
            }
        }
    }
}
