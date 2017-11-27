using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class MapParameters
    {
        internal int FramingLevel { get; private set; }
        internal int QuestionDrawDepth { get; private set; }
        internal int AnswerDrawDepth { get; private set; }
        internal int WrongChoiceDrawDepth { get; private set; }
        internal XmlCategorySelectionModeEnum CategorySelectionMode { get; private set; }
        internal List<XmlMapCategory> Categories { get; private set; }

        internal MapParameters(XmlMapParameters1 xmlMapParameters1)
        {
            FramingLevel = xmlMapParameters1.framingLevel;
            QuestionDrawDepth = 0;
            AnswerDrawDepth = Int32.Parse(xmlMapParameters1.answerDrawDepth);
            WrongChoiceDrawDepth = Int32.Parse(xmlMapParameters1.wrongChoiceDrawDepth);
            CategorySelectionMode = xmlMapParameters1.categorySelectionMode;
            Categories = xmlMapParameters1.category.ToList();
        }

        internal MapParameters(XmlMapParameters2 xmlMapParameters2)
        {
            FramingLevel = xmlMapParameters2.framingLevel;
            QuestionDrawDepth = Int32.Parse(xmlMapParameters2.questionDrawDepth);
            AnswerDrawDepth = Int32.Parse(xmlMapParameters2.answerDrawDepth);
            WrongChoiceDrawDepth = Int32.Parse(xmlMapParameters2.wrongChoiceDrawDepth);
            CategorySelectionMode = xmlMapParameters2.categorySelectionMode;
            Categories = xmlMapParameters2.category.ToList();
        }
    }
}
