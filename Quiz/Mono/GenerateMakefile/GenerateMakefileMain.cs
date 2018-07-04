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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Schemas;

namespace GenerateMakefile
{
    class GenerateMakefileMain
    {
        private static List<Rule> _ruleList = new List<Rule>();

        private static List<string> _cppGeneratedFileNames = new List<string>()
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

        private static Dictionary<string, string> _generationDirectories = new Dictionary<string, string>();

        private static void Main(string[] args)
        {
            Console.WriteLine("Quiz Makefile generation...");

            int result = setGenerationDirectories();

            if (result == 0)
            {
                AddObjectFilesLink();
                addCppGeneratedFilesCompilation();
                addCppFilesCompilation();
                addCppFileGeneration();
                addQuestionInstantiationBuild();
                addXsdGeneration();

                generateMakefile();
            }

            //Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();

            System.Environment.Exit(result);
        }

        private static int setGenerationDirectories()
        {
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
                            _generationDirectories.Add(
                                String.Format("{0}{1}{2}", quizData.parameters.questionnaireId, level.levelId, language.id),
                                Path.GetFileName(file));
                        }
                    }
                }
            }

            return 0;
        }

        private static void AddObjectFilesLink()
        {
            Rule mainRule = new Rule();
            mainRule.Target = "quiz";
            _ruleList.Add(mainRule);

            string[] files = Directory.GetFiles("../../../../NodeJS/produce_questions2", "*.cpp");
            List<string> fileList = new List<string>();
            foreach (string file in files) fileList.Add(Path.GetFileName(file));

            foreach (string directory in _generationDirectories.Keys)
            {
                Rule rule = new Rule();
                rule.Target = String.Format("NodeJS/produce_questions{0}.exe", directory);
                mainRule.Dependencies.Add(rule.Target);

                string command = String.Format("cd NodeJS/produce_questions2 && g++ -o ../produce_questions{0}.exe", directory);

                foreach (string file in fileList)
                {
                    rule.Dependencies.Add(String.Format("NodeJS/produce_questions2/obj/Release/{0}o", file.Substring(0, file.Length - 3)));
                    command += String.Format(" obj/Release/{0}o", file.Substring(0, file.Length - 3));
                }

                foreach (string file in _cppGeneratedFileNames)
                {
                    rule.Dependencies.Add(String.Format("NodeJS/produce_questions2/obj/Release/generated_code/{0}/{1}.o", directory, file));
                    command += String.Format(" obj/Release/generated_code/{0}/{1}.o", directory, file);
                }

                command += " -s";
                rule.Commands.Add(command);

                _ruleList.Add(rule);
            }
        }

        private static void addCppGeneratedFilesCompilation()
        {
            foreach (string directory in _generationDirectories.Keys)
            {
                foreach (string file in _cppGeneratedFileNames)
                {
                    string file2 = String.Format("{0}/{1}.", directory, file);

                    Rule rule = new Rule();
                    rule.Target = String.Format("NodeJS/produce_questions2/obj/Release/generated_code/{0}o", file2);
                    rule.Dependencies.Add(String.Format("NodeJS/produce_questions2/generated_code/{0}cpp", file2));

                    rule.Commands.Add(String.Format("mkdir -p NodeJS/produce_questions2/obj/Release/generated_code/{0}", directory));
                    rule.Commands.Add(String.Format(
                        "cd NodeJS/produce_questions2 && g++ -Wall -fexceptions -O2 -std=c++11 -c generated_code/{0}cpp -o obj/Release/generated_code/{0}o",
                        file2));

                    _ruleList.Add(rule);
                }
            }
        }

        private static void addCppFilesCompilation()
        {
            Dictionary<string, Dictionary<string, int>> dependencies = new Dictionary<string, Dictionary<string, int>>();

            string[] cppFiles = Directory.GetFiles("../../../../NodeJS/produce_questions2", "*.cpp");
            string[] hFiles = Directory.GetFiles("../../../../NodeJS/produce_questions2", "*.h");

            List<string> cppFileList = new List<string>();
            List<string> fileList = new List<string>();

            foreach (string file in cppFiles)
            {
                string name = Path.GetFileName(file);
                cppFileList.Add(name);
                fileList.Add(name);
            }

            foreach (string file in hFiles)
            {
                string name = Path.GetFileName(file);
                fileList.Add(name);
            }
                
            foreach (string file in fileList) dependencies.Add(file, new Dictionary<string, int>());
            foreach (string file in fileList) addIncludeDependencies(file, dependencies, new Dictionary<string, int>(), new List<string>());

            foreach (string file in cppFileList)
            {
                Rule rule = new Rule();
                rule.Target = String.Format("NodeJS/produce_questions2/obj/Release/{0}o", file.Substring(0, file.Length - 3));
                rule.Dependencies.Add(String.Format("NodeJS/produce_questions2/{0}", file));

                foreach (string dep in dependencies[file].Keys) rule.Dependencies.Add(String.Format("NodeJS/produce_questions2/{0}", dep));

                rule.Commands.Add("mkdir -p NodeJS/produce_questions2/obj/Release");
                rule.Commands.Add(String.Format("cd NodeJS/produce_questions2 && g++ -Wall -fexceptions -O2 -std=c++11 -c {0} -o obj/Release/{1}o",
                    file, file.Substring(0, file.Length - 3)));

                _ruleList.Add(rule);
            }
        }

        private static void addIncludeDependencies(string file, Dictionary<string, Dictionary<string, int>> dependencies, Dictionary<string, int> doneFiles, List<string> fileStack)
        {
            fileStack.Add(file);
            doneFiles.Add(file, 1);

            List<string> includes = getIncludeFiles(file);
            foreach (string include in includes)
            {
                if (dependencies.ContainsKey(include))
                {
                    foreach (string f in fileStack) dependencies[f][include] = 1;
                    if (!doneFiles.ContainsKey(include)) addIncludeDependencies(include, dependencies, doneFiles, fileStack);
                }
            }

            fileStack.RemoveAt(fileStack.Count - 1);
        }

        private static List<string> getIncludeFiles(string file)
        {
            List<string> includeFiles = new List<string>();
            StreamReader reader = new System.IO.StreamReader(String.Format("../../../../NodeJS/produce_questions2/{0}", file));

            while(true)
            {
                string line = reader.ReadLine();
                if (line == null) break;

                // Note: We do not handle the case where the 'include' directive is inside a comment
                if (line.Contains("#") && line.Contains("include"))
                {
                    int index = line.IndexOf('"');
                    if (index >= 0)
                    {
                        line = line.Substring(index + 1);
                        index = line.IndexOf('"');
                        if (index >= 0)
                        {
                            line = line.Substring(0, index);
                            includeFiles.Add(line);
                        }
                    }
                }
            }

            reader.Close();

            return includeFiles;
        }

        private static void addCppFileGeneration()
        {
            foreach (KeyValuePair<string, string> pair in _generationDirectories)
            {
                foreach (string fileName in _cppGeneratedFileNames)
                {
                    Rule rule = new Rule();
                    rule.Target = String.Format("NodeJS/produce_questions2/generated_code/{0}/{1}.cpp", pair.Key, fileName);
                    rule.Dependencies.Add("Mono/QuestionInstantiation/bin/Debug/QuestionInstantiation.exe");
                    rule.Dependencies.Add(String.Format("VisualStudio/QuestionInstantiation/{0}", pair.Value));
                    rule.Commands.Add("mkdir -p NodeJS/produce_questions2/generated_code");
                    rule.Commands.Add(String.Format(
                        "cd Mono/QuestionInstantiation/bin/Debug && mono QuestionInstantiation.exe ../../../../VisualStudio/QuestionInstantiation/{0}",
                        pair.Value));
                    _ruleList.Add(rule);
                }
            }
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

        private static void generateMakefile()
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
