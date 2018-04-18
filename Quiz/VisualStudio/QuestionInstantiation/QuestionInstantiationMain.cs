using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuestionInstantiation
{
    class QuestionInstantiationMain
    {
        static void Main(string[] args)
        {
            int result = 0;

            if (args.Length < 1)
            {
                Console.WriteLine("usage: QuestionInstantiation.exe path1 [path2 ...]");
                result = -1;
            }
            else
            {
                foreach (string path in args)
                {
                    Questionnaire questionnaire = new Questionnaire();
                    result = questionnaire.instantiate(path);
                    if (result != 0) break;
                }
            }

            //Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();

            System.Environment.Exit(result);
        }
    }
}
