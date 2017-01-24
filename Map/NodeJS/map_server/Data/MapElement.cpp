#include "MapElement.h"
#include "ElementName.h"
#include "Combinations.h"

#undef max
#include <limits>
#include <locale>
#include <codecvt>

namespace map_server
{
    MapElement::~MapElement()
    {
        std::map<std::string, std::vector<ElementName *> >::iterator it = _nameMap.begin();
        for (; it != _nameMap.end(); ++it)
        {
            int i, n = (*it).second.size();
            for (i = 0; i < n; ++i) delete (*it).second[i];
        }
    }

    void MapElement::loadCommon(mongo::BSONObj dbElement)
    {
        //_numericalId = dbElement.getIntField("num_id");
        _importance = dbElement.getField("importance").Double();

        std::string namesJson;

        mongo::BSONObj dbName = dbElement.getField("name").Obj();
        mongo::BSONObj dbShortName = dbElement.getField("short_name").Obj();

        int i, n = _iMap->getLanguageIdVectorPtr()->size();
        for (i = 0; i < n; ++i)
        {
            const char *languageId = (*_iMap->getLanguageIdVectorPtr())[i].c_str();
            std::string name = dbName.getStringField(languageId);
            std::string shortName = dbShortName.getStringField(languageId);

            std::vector<ElementName *> nameVector;
            addNames(name, nameVector);
            addNames(shortName, nameVector);

            _nameMap.insert(std::pair<std::string, std::vector<ElementName *> >(languageId, nameVector));

            if (namesJson.empty()) namesJson = "{\"";
            else namesJson += "\",\"";
            namesJson += std::string(languageId) + "\":\"" + name;
        }
        if (!namesJson.empty()) namesJson += "\"}";

        if (namesJson.empty()) namesJson = "undefined";
        _infoJson = "{\"names\":" + namesJson + "}";
    }

    void MapElement::addNames(const std::string& name, std::vector<ElementName *>& nameVector)
    {
        if (!name.empty())
        {
            if (!allowMultiline())
            {
                std::vector<std::string> lineVector;
                lineVector.push_back(name);
                nameVector.push_back(new ElementName(lineVector));
            }
            else
            {
                bool firstCharFound = false;
                int startIndex = 0;
                std::vector<std::string> wordVector;
                std::vector<int> wordSizeVector;

                std::wstring_convert<std::codecvt_utf8<wchar_t> > converter;

                int i, n = name.size();
                for (i = 0; i < n; ++i)
                {
                    if (i == n - 1 || (firstCharFound && (name[i] == '-' || name[i] == ' ') && name[i + 1] != '-' && name[i + 1] != ' '))
                    {
                        std::string word = name.substr(startIndex, i + 1 - startIndex);
                        wordVector.push_back(word);
                        wordSizeVector.push_back(converter.from_bytes(word).size());
                        startIndex = i + 1;
                    }
                    else if (name[i] != '-' && name[i] != ' ')
                    {
                        firstCharFound = true;
                    }
                }

                n = wordVector.size() - 1;
                int k;
                for (k = 0; k <= n; ++k)
                {
                    // The goal of this algorithm is:
                    // Find the best way to split a (n+1)-word string into k+1 lines.
                    // (The best split is the one that minimizes the text width.
                    // The text width is the width of the longest line)
                    // There are C(n, k) ways to split a (n+1)-word string into k+1 lines
                    // (where C(n, k) is the number of k-combinations from a set of n elements)
                    std::vector<std::string> bestLineVector;
                    int bestWidth = std::numeric_limits<int>::max();
                    int combinationCount = Combinations::getCount(n, k);
                    for (i = 0; i < combinationCount; ++i)
                    {
                        std::vector<std::string> lineVector;
                        int width = 0;
                        int index0 = 0;
                        int j;
                        for (j = 0; j <= k; ++j)
                        {
                            int index;
                            if (j < k) index = Combinations::get(n, k, i, j);
                            else index = n;

                            int w = 0;
                            std::string line;
                            int p;
                            for (p = index0; p <= index; ++p)
                            {
                                line += wordVector[p];
                                w += wordSizeVector[p];
                            }

                            while (line.back() == ' ')
                            {
                                line.pop_back();
                                --w;
                            }

                            lineVector.push_back(line);
                            index0 = index + 1;
                            if (w > width) width = w;
                        }

                        if (width < bestWidth)
                        {
                            bestWidth = width;
                            bestLineVector = lineVector;
                        }
                    }

                    nameVector.push_back(new ElementName(bestLineVector));
                }
            }
        }
    }
}
