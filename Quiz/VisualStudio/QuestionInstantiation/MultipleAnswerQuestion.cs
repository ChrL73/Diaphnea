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
    class MultipleAnswerQuestion
    {
        private readonly Text _questionText;
        private readonly Dictionary<Choice, int> _choiceDictionary = new Dictionary<Choice, int>();
        private readonly List<Choice> _choiceList = new List<Choice>();
        private readonly Choice _excludedChoice;
        private readonly Element _questionElement;
        //private readonly XmlMultipleAnswerProximityCriterionEnum _proximityCriterion;
        private readonly ChoiceCommentModeEnum _choiceCommentMode;

        internal MultipleAnswerQuestion(Text questionText, Choice excludedChoice, Element questionElement, /*XmlMultipleAnswerProximityCriterionEnum proximityCriterion,*/
                                        ChoiceCommentModeEnum choiceCommentMode)
        {
            _questionText = questionText;
            _excludedChoice = excludedChoice;
            _questionElement = questionElement;
            //_proximityCriterion = proximityCriterion;
            _choiceCommentMode = choiceCommentMode;
        }

        internal Text QuestionText { get { return _questionText; } }
        internal Element QuestionElement { get { return _questionElement; } }
        internal List<Choice> ChoiceList { get { return _choiceList; } }
        internal ChoiceCommentModeEnum ChoiceCommentMode { get { return _choiceCommentMode; } }
        internal Choice ExcludedChoice { get { return _excludedChoice; } }

        internal void addChoice(Choice choice)
        {
            _choiceDictionary[choice] = 0;
            _choiceList.Add(choice);
        }

        internal int getChoiceCount()
        {
            return _choiceList.Count;
        }

        internal Choice getChoice(int i)
        {
            return _choiceList.ElementAt(i);
        }
    }
}
