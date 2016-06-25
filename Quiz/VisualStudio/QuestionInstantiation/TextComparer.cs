using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class TextComparer : IComparer<Text>
    {
        // 2 'Text' are equal when they have the same translation in at least one language
        public int Compare(Text x, Text y)
        {
            foreach (string language in x.LanguageList)
            {
                if (x.getText(language) == y.getText(language)) return 0;
            }

            return x.TextId.CompareTo(y.TextId);
        }
    }
}
