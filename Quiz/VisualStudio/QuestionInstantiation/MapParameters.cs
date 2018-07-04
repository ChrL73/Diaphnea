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
using Schemas;

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
