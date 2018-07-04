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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CheckSourceFiles
{
    class CheckSourceFilesMain
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("usage: CheckSourceFile.exe path searchPattern");
                return;
            }

            Console.WriteLine(args[0]);
            string[] files = Directory.GetFiles(args[0], args[1], SearchOption.AllDirectories);

            foreach (string filePath in files)
            {
                if (!filePath.Contains("TemporaryGeneratedFile") && !filePath.Contains("XmlQuizData.cs") && !filePath.Contains("node_modules"))
                {
                    Console.WriteLine(filePath);

                    string line;
                    int counter = 0;
                    System.IO.StreamReader file = new System.IO.StreamReader(filePath);
                    while ((line = file.ReadLine()) != null)
                    {
                        ++counter;
                        if (line.Contains("\t")) Console.WriteLine(String.Format("line {0}: {1}", counter, line));
                    }

                    file.Close();
                    Console.WriteLine();
                }

            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
