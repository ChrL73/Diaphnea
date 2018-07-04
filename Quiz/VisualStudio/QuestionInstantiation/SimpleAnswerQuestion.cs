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
    class SimpleAnswerQuestion
    {
        private readonly Text _questionText;
        private readonly Choice _choice;
        private readonly Choice _excludedChoice;
        private readonly Element _questionElement;
        private readonly XmlSimpleAnswerProximityCriterionEnum _proximityCriterion;

        internal SimpleAnswerQuestion(Text questionText, Choice choice, Choice excludedChoice, Element questionElement, XmlSimpleAnswerProximityCriterionEnum proximityCriterion)
        {
            _questionText = questionText;
            _choice = choice;
            _excludedChoice = excludedChoice;
            _questionElement = questionElement;
            _proximityCriterion = proximityCriterion;
        }

        internal Text QuestionText { get { return _questionText; } }
        internal Choice Choice { get { return _choice; } }
        internal Element QuestionElement { get { return _questionElement; } }
        internal Choice ExcludedChoice { get { return _excludedChoice; } }
        internal XmlSimpleAnswerProximityCriterionEnum ProximityCriterion { get { return _proximityCriterion; } }
    }
}
