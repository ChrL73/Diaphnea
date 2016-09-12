using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class MessageLogger
    {
        static MessageLogger()
        {
            LogFileName = "log.txt";
            ConsoleLogLevel = XmlLogLevelEnum.MESSAGE;
            FileLogLevel = XmlLogLevelEnum.MESSAGE;

            if (File.Exists(LogFileName)) File.Delete(LogFileName);
        }

        internal static string LogFileName { get; set; }
        internal static XmlLogLevelEnum ConsoleLogLevel { get; set; }
        internal static XmlLogLevelEnum FileLogLevel { get; set; }

        internal static void addMessage(XmlLogLevelEnum logLevel, string message)
        {
            addFileMessage(logLevel, message);
            addConsoleMessage(logLevel, message);
        }

        private static void addFileMessage(XmlLogLevelEnum logLevel, string message)
        {
            if (logLevel == XmlLogLevelEnum.NO_LOG || logLevel > FileLogLevel) return;

            using (StreamWriter file = new StreamWriter(LogFileName, true))
            {
                if (logLevel == XmlLogLevelEnum.ERROR) file.Write("Error: ");
                else if (logLevel == XmlLogLevelEnum.WARNING) file.Write("Warning: ");
                else file.Write("Message: ");

                file.WriteLine(message);
            }
        }

        private static void addConsoleMessage(XmlLogLevelEnum logLevel, string message)
        {
            if (logLevel == XmlLogLevelEnum.NO_LOG || logLevel > ConsoleLogLevel) return;

            if (logLevel == XmlLogLevelEnum.ERROR) Console.Write("Error: ");
            else if (logLevel == XmlLogLevelEnum.WARNING) Console.Write("Warning: ");
            else Console.Write("Message: ");

            Console.WriteLine(message);
        }
    }
}
