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

namespace QuestionInstantiation
{
    class RelationOrderQuestion
    {
        private readonly List<Choice> _choiceList = new List<Choice>();
        private readonly SortedDictionary<Text, Choice> _choiceDictionary = new SortedDictionary<Text, Choice>(new TextComparer());
        private readonly Text _questionText;
        private readonly Element _questionElement;

        internal RelationOrderQuestion(Text questionText, Element questionElement)
        {
            _questionText = questionText;
            _questionElement = questionElement;
        }

        internal Text QuestionText { get { return _questionText; } }
        internal Element QuestionElement { get { return _questionElement; } }
        internal List<Choice> ChoiceList { get { return _choiceList; } }

        internal int ChoiceCount
        {
            get { return _choiceList.Count; }
        }

        internal void addChoice(Choice choice)
        {
            Text choiceText = choice.AttributeValue.Value;
            if (!_choiceDictionary.ContainsKey(choiceText))
            {
                _choiceDictionary.Add(choiceText, choice);
                _choiceList.Add(choice);
            }
            else
            {
                Choice choiceToDelete = _choiceDictionary[choiceText];
                int i, n = _choiceList.Count;
                for (i = 0; i < n; ++i)
                {
                    if (_choiceList[i] == choiceToDelete)
                    {
                        _choiceList.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
