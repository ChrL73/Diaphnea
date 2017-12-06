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
        internal XmlMapSubParameters QuestionParameters { get; private set; }
        internal XmlMapSubParameters AnswerParameters { get; private set; }
        internal XmlMapSubParameters WrongChoiceParameters { get; private set; }
        internal XmlAnswerSelectionModeEnum? AnswerSelectionMode { get; private set; }

        internal MapParameters(XmlMapParameters1 xmlMapParameters1)
        {
            FramingLevel = xmlMapParameters1.framingLevel;
            QuestionParameters = null;
            AnswerParameters = xmlMapParameters1.answer;
            WrongChoiceParameters = xmlMapParameters1.wrongChoice;
            AnswerSelectionMode = null;
        }

        internal MapParameters(XmlMapParameters2 xmlMapParameters2)
        {
            FramingLevel = xmlMapParameters2.framingLevel;
            QuestionParameters = xmlMapParameters2.question;
            AnswerParameters = xmlMapParameters2.answer;
            WrongChoiceParameters = xmlMapParameters2.wrongChoice;
            AnswerSelectionMode = null;
        }

        internal MapParameters(XmlMapParameters3 xmlMapParameters3)
        {
            FramingLevel = xmlMapParameters3.framingLevel;
            QuestionParameters = xmlMapParameters3.question;
            WrongChoiceParameters = xmlMapParameters3.wrongChoice;

            AnswerParameters = new XmlMapSubParameters()
            {
                drawDepth = xmlMapParameters3.answer.drawDepth,
                categorySelectionMode = xmlMapParameters3.answer.categorySelectionMode,
                category = xmlMapParameters3.answer.category,
                framingMode = xmlMapParameters3.answer.framingMode
            };

            AnswerSelectionMode = xmlMapParameters3.answer.answerSelectionMode;
        }
    }
}
