#include "MapElement.h"

namespace map_server
{
    void MapElement::loadCommon(mongo::BSONObj dbElement)
    {
        std::string namesJson;

        mongo::BSONObj dbName = dbElement.getField("name").Obj();
        mongo::BSONObj dbShortName = dbElement.getField("short_name").Obj();

        int i, n = _iMap->getLanguageIdVectorPtr()->size();
        for (i = 0; i < n; ++i)
        {
            const char *languageId = (*_iMap->getLanguageIdVectorPtr())[i].c_str();
            const char *name = dbName.getStringField(languageId);
            const char *shortName = dbShortName.getStringField(languageId);
            std::pair<std::string, std::string> names(name, shortName);
            _nameMap.insert(std::pair<std::string, std::pair<std::string, std::string> >(languageId, names));

            if (namesJson.empty()) namesJson = "{\"";
            else namesJson += "\",\"";
            namesJson += std::string(languageId) + "\":\"" + name;
        }
        if (!namesJson.empty()) namesJson += "\"}";

        if (namesJson.empty()) namesJson = "undefined";
        _infoJson = "{\"names\":" + namesJson + "}";
    }
}
