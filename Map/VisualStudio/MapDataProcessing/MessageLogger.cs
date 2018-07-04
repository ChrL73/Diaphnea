/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

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
