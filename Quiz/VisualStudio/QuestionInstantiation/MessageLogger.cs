using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    enum LogLevelEnum
    {
        NO_LOG = 0,
        FATAL_ERROR = 1,
        WARNING = 2,
        MESSAGE = 3
    }

    class MessageLogger
    {
        static MessageLogger()
        {
            ConsoleLogLevel = LogLevelEnum.MESSAGE;
            FileLogLevel = LogLevelEnum.MESSAGE;
        }

        internal static LogLevelEnum ConsoleLogLevel { get; set; }
        internal static LogLevelEnum FileLogLevel { get; set; }

        internal static void addMessage(LogLevelEnum logLevel, string message)
        {
            addFileMessage(logLevel, message);
            addConsoleMessage(logLevel, message);
        }

        private static void addFileMessage(LogLevelEnum logLevel, string message)
        {
            if (logLevel == LogLevelEnum.NO_LOG || logLevel > FileLogLevel) return;

            using (StreamWriter file = new StreamWriter("log.txt", true))
            {
                if (logLevel == LogLevelEnum.FATAL_ERROR) file.Write("Error: ");
                else if (logLevel == LogLevelEnum.WARNING) file.Write("Warning: ");
                else file.Write("Message: ");

                file.WriteLine(message);
            }
        }

        private static void addConsoleMessage(LogLevelEnum logLevel, string message)
        {
            if (logLevel == LogLevelEnum.NO_LOG || logLevel > ConsoleLogLevel) return;

            if (logLevel == LogLevelEnum.FATAL_ERROR) Console.Write("Error: ");
            else if (logLevel == LogLevelEnum.WARNING) Console.Write("Warning: ");
            else Console.Write("Message: ");

            Console.WriteLine(message);
        }
    }
}
