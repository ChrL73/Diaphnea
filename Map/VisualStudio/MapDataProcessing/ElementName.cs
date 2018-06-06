using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace MapDataProcessing
{
    class ElementName
    {
        private static Dictionary<string, int> _languageDictionary = new Dictionary<string, int>();

        internal static void reset()
        {
            _languageDictionary.Clear();
        }

        private readonly SortedDictionary<string, string> _dictionary = new SortedDictionary<string, string>();

        internal ElementName(XmlName[] name)
        {
            if (name != null)
            {
                foreach (XmlName translation in name)
                {
                    string languageId = translation.language.ToString();
                    _languageDictionary[languageId] = 0;
                    _dictionary.Add(languageId, translation.text);
                }
            }
        }

        internal ElementName(XmlName[] name, XmlName[] suffix)
        {
            if (name != null)
            {
                foreach (XmlName translation in name)
                {
                    string languageId = translation.language.ToString();
                    _languageDictionary[languageId] = 0;
                    _dictionary.Add(languageId, translation.text);
                }

                if (suffix != null)
                {
                    foreach (XmlName translation in suffix)
                    {
                        string languageId = translation.language.ToString();
                        if (_languageDictionary.ContainsKey(languageId)) _dictionary[languageId] += translation.text;
                    }
                }
            }
        }

        internal SortedDictionary<string, string> Dictionary { get { return _dictionary;} }

        internal string getJson()
        {
            List<string> jsonList = new List<string>();
            foreach (string languageId in _languageDictionary.Keys)
            {
                string name;
                if (!_dictionary.TryGetValue(languageId, out name)) name = "";
                jsonList.Add(String.Format("\"{0}\":\"{1}\"", languageId, name));
            }

            return String.Format("{{{0}}}", String.Join(",", jsonList));
        }
    }
}
