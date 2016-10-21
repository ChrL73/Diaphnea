#include "PointElement.h"

namespace map_server
{
    void PointElement::load(void)
    {
        _loaded = true;
        std::string namesJson;

        auto cursor = _connectionPtr->query("diaphnea.point_elements", MONGO_QUERY("_id" << _mongoId), 1);
        if (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();

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
}
