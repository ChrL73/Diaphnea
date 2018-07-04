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
    class Text
    {
        private SortedDictionary<string, string> _textDictionary = new SortedDictionary<string, string>();
        private string _textId;

        internal void setText(string languageId, string text)
        {
            _textDictionary[languageId] = text;

            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> pair in _textDictionary)
            {
                list.Add(pair.Key);
                list.Add(pair.Value);
            }
            
            _textId = String.Join("|", list);
        }

        internal string getText(string languageId)
        {
            string text;
            if (_textDictionary.TryGetValue(languageId, out text)) return text;
            return null;
        }

        internal string TextId { get { return _textId; } }

        internal IEnumerable<string> LanguageList { get { return _textDictionary.Keys; } }

        private static Dictionary<string, int> _completedTranslationDictionary;
        internal static void setCompletedTranslationDictionary(QuizData quizData)
        {
            _completedTranslationDictionary = new Dictionary<string, int>();
            foreach (XmlLanguage xmlLanguage in quizData.XmlQuizData.parameters.languageList)
            {
                if (xmlLanguage.status == XmlLanguageStatusEnum.TRANSLATION_COMPLETED) _completedTranslationDictionary[xmlLanguage.id.ToString()] = 0;
            }
        }

        internal static IEnumerable<string> CompletedTranslationLanguages
        {
            get { return _completedTranslationDictionary.Keys; }
        }

        internal Double? getAsDouble()
        {
            if (_textDictionary.Count() == 0) return null;

            Double? d = null;
            foreach (string s in _textDictionary.Values)
            {
                Double res;
                if (!Double.TryParse(s, out res))
                {
                    return null;
                }
                else
                {
                    if (d != null)
                    {
                        if (d != res) return null;
                    }
                    else
                    {
                        d = res;
                    }
                }
            }

           return d;
        }

        internal static Text emptyText(QuizData quizData)
        {
            Text result = new Text();
            foreach (XmlLanguage xmlLanguage in quizData.XmlQuizData.parameters.languageList.Where(x => x.status == XmlLanguageStatusEnum.TRANSLATION_COMPLETED))
            {
                result.setText(xmlLanguage.id.ToString(), "");
            }
            return result;
        }

        internal static Text fromTextList(IEnumerable<Text> textList, QuizData quizData)
        {
            Text result = new Text();
            foreach (XmlLanguage xmlLanguage in quizData.XmlQuizData.parameters.languageList.Where(x => x.status == XmlLanguageStatusEnum.TRANSLATION_COMPLETED))
            {
                List<string> list = new List<string>();
                foreach (Text text in textList) list.Add(text.getText(xmlLanguage.id.ToString()));
                string str = String.Join(", ", list);
                result.setText(xmlLanguage.id.ToString(), str);
            }
            return result;
        }
    }
}
