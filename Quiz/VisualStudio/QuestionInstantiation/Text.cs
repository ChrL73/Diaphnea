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
            
            _textId = String.Join("#", list);
        }

        internal string getText(string languageId)
        {
            string text;
            if (_textDictionary.TryGetValue(languageId, out text)) return text;
            return null;
        }

        internal string TextId { get { return _textId; } }

        internal BsonDocument getBsonDocument()
        {
            BsonDocument textDocument = new BsonDocument();

            foreach (KeyValuePair<string, string> pair in _textDictionary)
            {
                textDocument.Add(new BsonDocument() { { pair.Key, pair.Value } });
            }

            return textDocument;
        }
    }
}
