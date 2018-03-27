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
            if (args.Length < 1)
            {
                Console.WriteLine("usage: CheckSourceFile.exe path");
                return;
            }

            Console.WriteLine(args[0]);
            string[] files = Directory.GetFiles(args[0], "*.cs", SearchOption.AllDirectories);

            foreach (string filePath in files)
            {
                if (!filePath.Contains("TemporaryGeneratedFile") && !filePath.Contains("XmlQuizData.cs"))
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
