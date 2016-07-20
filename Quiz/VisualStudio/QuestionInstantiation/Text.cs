using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (xmlLanguage.status == XmlLanguageStatusEnum.TRANSLATION_COMPLETED) _completedTranslationDictionary.Add(xmlLanguage.id.ToString(), 0);
            }
        }

        internal string getAsNumber()
        {
            if (_textDictionary.Count() == 0) return null;

            bool ok = true;
            Double? d = null;
            foreach (string s in _textDictionary.Values)
            {
                Double res;
                if (!Double.TryParse(s, out res))
                {
                    ok = false;
                }
                else
                {
                    if (d != null)
                    {
                        if (d != res) ok = false;
                    }
                    else
                    {
                        d = res;
                    }
                }
            }

            if (ok) return _textDictionary.Values.First();

            ok = true;
            Int32? i = null;
            foreach (string s in _textDictionary.Values)
            {
                Int32 res;
                if (!Int32.TryParse(s, out res))
                {
                    ok = false;
                }
                else
                {
                    if (i != null)
                    {
                        if (i != res) ok = false;
                    }
                    else
                    {
                        i = res;
                    }
                }
            }

            if (ok) return _textDictionary.Values.First();
            return null;
        }

        internal BsonDocument getBsonDocument()
        {
            BsonDocument textDocument = new BsonDocument();

            foreach (KeyValuePair<string, string> pair in _textDictionary)
            {
                if (_completedTranslationDictionary.ContainsKey(pair.Key)) textDocument.AddRange(new BsonDocument() { { pair.Key, pair.Value == null ? "" : pair.Value } });
            }

            return textDocument;
        }
    }
}
