using System;
using System.Collections.Generic;
using System.IO;

namespace GenerateMakefile
{
    class GenerateMakefileMain
    {
        private static string _baseDir = "../../../../";
        private static List<Rule> _ruleList = new List<Rule>();

        private static void Main(string[] args)
        {
            Rule rule = new Rule();
            rule.Target = "VisualStudio/QuestionInstantiation/XmlQuizData.cs";
            rule.Dependencies.Add("VisualStudio/QuestionInstantiation/XmlQuizData.xsd");
            rule.Commands.Add("xsd VisualStudio/QuestionInstantiation/XmlQuizData.xsd /classes /outputdir:VisualStudio/QuestionInstantiation/");
            _ruleList.Add(rule);

            generate();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void generate()
        {
            foreach (Rule rule in _ruleList)
            {
                string path = String.Format("{0}Makefile", _baseDir);

                using (StreamWriter file = new StreamWriter(path, false))
                {
                    file.Write(String.Format("{0}: ", rule.Target));
                    file.WriteLine(String.Join(" ", rule.Dependencies));
                    foreach (string command in rule.Commands) file.WriteLine("\t{0}", command);
                }
            }
        }
    }
}
