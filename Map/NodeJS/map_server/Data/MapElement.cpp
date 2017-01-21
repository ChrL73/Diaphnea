#include "MapElement.h"
#include "ElementName.h"

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
            bool firstCharFound = false;
            int startIndex = 0;
            std::vector<std::string> fragmentVector;

            int i, n = name.size();
            for (i = 0; i < n; ++i)
            {
                if (i == n - 1 || (firstCharFound && (name[i] == '-' || name[i] == ' ') && name[i + 1] != '-' && name[i + 1] != ' '))
                {
                    fragmentVector.push_back(name.substr(startIndex, i + 1 - startIndex));
                    startIndex = i + 1;
                }
                else
                {
                    firstCharFound = true;
                }
            }

            std::vector<std::string> lineVector;
            /*lineVector.push_back(name);
            nameVector.push_back(new ElementName(lineVector));*/
        }
    }
}
