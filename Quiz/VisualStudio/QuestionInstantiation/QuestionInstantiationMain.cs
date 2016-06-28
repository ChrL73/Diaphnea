
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
            if (args.Length < 1)
            {
                Console.WriteLine("usage: QuestionInstantiation path1 [path2 ...]");
                return;
            }

            foreach (string path in args)
            {
                Questionnaire questionnaire = new Questionnaire();
                questionnaire.instantiate(path);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
