#include "MapElement.h"

namespace map_server
{
    void MapElement::loadCommon(mongo::BSONObj dbElement)
    {
        std::string namesJson;

        mongo::BSONObj dbName = dbElement.getField("name").Obj();
        int i, n = _languageIdVectorPtr->size();
        for (i = 0; i < n; ++i)
        {
            const char *languageId = (*_languageIdVectorPtr)[i].c_str();
            const char *elementName = dbName.getStringField(languageId);
            _nameMap.insert(std::pair<std::string, std::string>(languageId, elementName));

            if (namesJson.empty()) namesJson = "{\"";
            else namesJson += "\",\"";
            namesJson += std::string(languageId) + "\":\"" + elementName;
        }
        if (!namesJson.empty()) namesJson += "\"}";

        if (namesJson.empty()) namesJson = "undefined";
        _infoJson = "{\"names\":" + namesJson + "}";
    }
}
