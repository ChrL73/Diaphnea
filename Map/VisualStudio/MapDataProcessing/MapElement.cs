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

namespace MapDataProcessing
{
    abstract class MapElement
    {
        private readonly String _id;
        protected readonly MapData _mapData;
        private readonly ElementName _name;
        private readonly ElementName _shortName;
        private readonly double _importance;
        private readonly List<Look> _looks = new List<Look>();
        private readonly Category _category;

        private readonly Dictionary<MapElement, int> _linkedElements1 = new Dictionary<MapElement, int>();
        private readonly Dictionary<MapElement, int> _linkedElements2 = new Dictionary<MapElement, int>();
        internal Dictionary<MapElement, int> LinkedElements1 { get { return _linkedElements1; } }
        internal Dictionary<MapElement, int> LinkedElements2 { get { return _linkedElements2; } }
        private readonly Dictionary<string, List<List<string>>> _nameDictionary = new Dictionary<string, List<List<string>>>();

        internal double Importance { get { return _importance; } }
        internal List<Look> Looks { get { return _looks; } }

        internal Dictionary<string, List<List<string>>> NameDictionary { get { return _nameDictionary; } }

        internal MapElement(String id, MapData mapData, XmlName[] name, XmlName[] shortName, double importance, List<string> lookIds, string categoryId)
        {
            _id = id;
            _mapData = mapData;
            _name = new ElementName(name);
            _shortName = new ElementName(shortName);
            _importance = importance;
            _category = mapData.getCategory(categoryId);

            foreach (string lookId in lookIds) _looks.Add(mapData.getLook(lookId));

            if (_name != null) addName(_name);
            if (_shortName != null) addName(_shortName);
        }

        abstract protected bool allowMultiline();
        abstract internal int addKmlFile(String path);
        abstract internal int formParts1();
        abstract internal int formParts2();

        internal string Id { get { return _id; } }
        protected MapData MapData { get { return _mapData; } }
        internal Category Category { get { return _category; } }

        private void addName(ElementName name)
        {
            foreach (KeyValuePair<string, string> pair in name.Dictionary)
            {
                if (pair.Value.Length > 0) addName(pair.Key, pair.Value);
            }
        }

        private void addName(string languageId, string name)
        {
            if (!allowMultiline())
            {
                List<string> lineList = new List<string>();
                lineList.Add(name);

                if (!_nameDictionary.ContainsKey(languageId)) _nameDictionary.Add(languageId, new List<List<string>>());
                _nameDictionary[languageId].Add(lineList);
            }
            else
            {
                bool firstCharFound = false;
                int startIndex = 0;
                List<string> wordList = new List<string>();
                List<int> wordSizeList = new List<int>();

                int i, n = name.Length;
                for (i = 0; i < n; ++i)
                {
                    if (i == n - 1 || (firstCharFound && (name[i] == '-' || name[i] == ' ') && name[i + 1] != '-' && name[i + 1] != ' '))
                    {
                        string word = name.Substring(startIndex, i + 1 - startIndex);
                        wordList.Add(word);
                        wordSizeList.Add(word.Length);
                        startIndex = i + 1;
                    }
                    else if (name[i] != '-' && name[i] != ' ')
                    {
                        firstCharFound = true;
                    }
                }

                n = wordList.Count() - 1;
                int k;
                for (k = 0; k <= n; ++k)
                {
                    // The goal of this algorithm is:
                    // Find the best way to split a (n+1)-word string into k+1 lines.
                    // (The best split is the one that minimizes the text width.
                    // The text width is the width of the longest line)
                    // There are C(n, k) ways to split a (n+1)-word string into k+1 lines
                    // (where C(n, k) is the number of k-combinations from a set of n elements)
                    List<string> bestLineList = null;
                    int bestWidth = Int32.MaxValue;
                    int combinationCount = Combinations.getCount(n, k);
                    for (i = 0; i < combinationCount; ++i)
                    {
                        List<string> lineList = new List<string>();
                        int width = 0;
                        int index0 = 0;
                        int j;
                        for (j = 0; j <= k; ++j)
                        {
                            int index;
                            if (j < k) index = Combinations.get(n, k, i, j);
                            else index = n;

                            int w = 0;
                            string line = "";
                            int p;
                            for (p = index0; p <= index; ++p)
                            {
                                line += wordList[p];
                                w += wordSizeList[p];
                            }

                            while (line[line.Length - 1] == ' ')
                            {
                                line = line.Remove(line.Length - 1);
                                --w;
                            }

                            lineList.Add(line);
                            index0 = index + 1;
                            if (w > width) width = w;
                        }

                        if (width < bestWidth)
                        {
                            bestWidth = width;
                            bestLineList = lineList;
                        }
                    }

                    if (!_nameDictionary.ContainsKey(languageId)) _nameDictionary.Add(languageId, new List<List<string>>());
                    _nameDictionary[languageId].Add(bestLineList);
                }
            }
        }

        internal string getInfoJson()
        {
            List<string> linkedElements1Array = new List<string>();
            foreach (MapElement linkedElement in _linkedElements1.Keys) linkedElements1Array.Add(String.Format("\"{0}\"", linkedElement.Id));
            string linkedElements1 = String.Join(",", linkedElements1Array);

            List<string> linkedElements2Array = new List<string>();
            foreach (MapElement linkedElement in _linkedElements2.Keys) linkedElements2Array.Add(String.Format("\"{0}\"", linkedElement.Id));
            string linkedElements2 = String.Join(",", linkedElements2Array);

            string tmp = String.Format("{{\"names\":{0},\"linkedElements1\":[{1}],\"linkedElements2\":[{2}],\"category\":{3}}}",
                _name.getJson(), linkedElements1, linkedElements2, _category.Id);

            return tmp;
        }

        abstract internal int generateCode(CodeGenerator codeGenerator);
    }
}
