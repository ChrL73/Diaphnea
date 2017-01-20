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
            const char *name = dbName.getStringField(languageId);
            const char *shortName = dbShortName.getStringField(languageId);

            std::vector<ElementName *> nameVector;
            std::vector<std::string> lineVector;
            lineVector.push_back(name);
            nameVector.push_back(new ElementName(lineVector));
            lineVector.clear();
            lineVector.push_back(shortName);
            nameVector.push_back(new ElementName(lineVector));

            _nameMap.insert(std::pair<std::string, std::vector<ElementName *> >(languageId, nameVector));

            if (namesJson.empty()) namesJson = "{\"";
            else namesJson += "\",\"";
            namesJson += std::string(languageId) + "\":\"" + name;
        }
        if (!namesJson.empty()) namesJson += "\"}";

        if (namesJson.empty()) namesJson = "undefined";
        _infoJson = "{\"names\":" + namesJson + "}";
    }
}
