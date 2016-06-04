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
    class QuestionInstantiationMain
    {
        private static QuizData _quizData;
        private static List<Level> _levelList = new List<Level>();

        static void Main(string[] args)
        {
            int result = 0;

            if (result == 0) result = loadData(args);
            if (result == 0) result = instantiateQuestions();

            if (result == 0) Console.WriteLine("Question instantitation terminated successfully");
            else Console.WriteLine("Question instantitation terminated with errors");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static int loadData(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No configuration file provided");
                return -1;
            }

            XmlReader reader;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;

            try
            {
                reader = XmlReader.Create(args[0], settings);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail to open file {0}", args[0]);
                Console.WriteLine(e.Message);
                return -1;
            }

            Console.WriteLine("Reading configuration file " + args[0] + " ...");

            XmlSerializer serializer = new XmlSerializer(typeof(XmlQuizData));
            XmlQuizData quizData = null;

            try
            {
                quizData = (XmlQuizData)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail to deserialize content of file {0}", args[0]);
                Console.WriteLine(e.Message);
                if (e.InnerException != null) Console.WriteLine(e.InnerException.Message);
                return -1;
            }

            reader.Close();

            _quizData =  new QuizData(args[0], quizData);

            MessageLogger.LogFileName = quizData.configuration.logFileName;
            MessageLogger.FileLogLevel = quizData.configuration.fileLogLevel;
            MessageLogger.ConsoleLogLevel = quizData.configuration.consoleLogLevel;

            return 0;
        }

        static int instantiateQuestions()
        {
            foreach(XmlLevel xmlLevel in _quizData.XmlQuizData.levelList)
            {
                Level level = new Level();
                if (level.initialize(_quizData, xmlLevel) != 0) return -1;
                _levelList.Add(level);
            }

            return 0;
        }
    }
}
