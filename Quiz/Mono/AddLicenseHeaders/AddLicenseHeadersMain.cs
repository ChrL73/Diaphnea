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

namespace AddLicenseHeaders
{
    class AddLicenseHeadersMain
    {
        private static readonly List<string> _patternList = new List<string>() { "*.cpp", "*.h", "*.cs", "*.js" };
        private static readonly string _beginLine = "/* --------------------------------------------------------------------";
        private static readonly string _endLine   = " * -------------------------------------------------------------------- */";
        private static readonly string _prefix = " * ";

        private static void Main(string[] args)
        {
            string[] licenseHeaderLines0 = null;
            string licenseTextPath = "../../LicenseHeader.txt";
            try
            {
                licenseHeaderLines0 = File.ReadAllLines(licenseTextPath);
            }
            catch (Exception)
            {
                Console.WriteLine(String.Format("Failed to read file ({0})", licenseTextPath));
                Environment.Exit(-1);
            }

            List<string> licenseHeaderLines = new List<string>();
            foreach (string line in licenseHeaderLines0)
            {
                string line2 = _prefix + line;
                licenseHeaderLines.Add(line2.TrimEnd(' '));
            }

            int totalUpdated = 0;
            int totalUpToDate = 0;
            int totalIgnored = 0;

            foreach (string pattern in _patternList)
            {
                int updated = 0;
                int upToDate = 0;
                int ignored = 0;

                foreach (string filePath in Directory.EnumerateFiles("../../../../../", pattern, SearchOption.AllDirectories))
                {
                    if (!ignoreFile(filePath))
                    {
                        //Console.WriteLine(filePath);
                        string[] lines0 = File.ReadAllLines(filePath);

                        int beginIndex = Array.IndexOf(lines0, _beginLine);
                        int endIndex = -1;
                        if (beginIndex != -1) endIndex = Array.IndexOf(lines0, _endLine);

                        bool bUpToDate = false;
                        if (endIndex != -1)
                        {
                            bUpToDate = true;
                            int i;
                            for (i = beginIndex + 1; i < endIndex; ++i)
                            {
                                if (i - beginIndex - 1 >= licenseHeaderLines.Count() || lines0[i] != licenseHeaderLines[i - beginIndex - 1])
                                {
                                    bUpToDate = false;
                                    break;
                                }
                            }
                        }

                        if (bUpToDate)
                        {
                            ++totalUpToDate;
                            ++upToDate;
                        }
                        else
                        {
                            List<string> lines1 = new List<string>();

                            if (endIndex == -1)
                            {
                                lines1.Add(_beginLine);
                                foreach (string line in licenseHeaderLines) lines1.Add(line);
                                lines1.Add(_endLine);
                                lines1.Add("");
                                foreach (string line in lines0) lines1.Add(line);
                            }
                            else
                            {
                                int i;
                                for (i = 0; i < beginIndex; ++i) lines1.Add(lines0[i]);
                                if (lines1.Count() != 0 && lines1[lines1.Count() - 1] != "") lines1.Add("");
                                lines1.Add(_beginLine);
                                foreach (string line in licenseHeaderLines) lines1.Add(line);
                                lines1.Add(_endLine);
                                if (lines0.Count() > endIndex + 1 && lines0[endIndex + 1] != "") lines1.Add("");
                                for (i = endIndex + 1; i < lines0.Count(); ++i) lines1.Add(lines0[i]);
                            }

                            using (TextWriter tw = new StreamWriter(filePath))
                            {
                                foreach (string line in lines1) tw.WriteLine(line);
                            }

                            ++totalUpdated;
                            ++updated;
                        }
                    }
                    else
                    {
                        ++totalIgnored;
                        ++ignored;
                    }
                }

                Console.WriteLine(String.Format("{0}: {1} updated, {2} up to date, {3} ignored", pattern, updated, upToDate, ignored));
                //Console.WriteLine();
            }

            Console.WriteLine(String.Format("Total: {0} updated, {1} up to date, {2} ignored)", totalUpdated, totalUpToDate, totalIgnored));
        }

        private static bool ignoreFile(string filePath)
        {
            return filePath.Contains("/generated_code/")
                || filePath.Contains("/node_modules/")
                || filePath.Contains("/Experiments/")
                || filePath.Contains("/obj/")
                || filePath.Contains("/jquery.min.js")
                || filePath.Contains("/public/static/js/")
                || filePath.Contains("/React/quiz/build/");
        }
    }
}
