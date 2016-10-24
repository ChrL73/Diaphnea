#include "Map.h"
#include "PointElement.h"
#include "PolygonElement.h"
#include "LineElement.h"
#include "PointLook.h"
#include "LineLook.h"
#include "PolygonLook.h"

namespace map_server
{
	Map::~Map()
	{
		std::map<std::string, MapElement *>::iterator it = _elementMap.begin();
		for (; it != _elementMap.end(); ++it) delete (*it).second;
	}

    MapElement *Map::getElement(const std::string& id)
    {
        std::map<std::string, MapElement *>::iterator elementIt = _elementMap.find(id);
        if (elementIt == _elementMap.end()) return 0;

        MapElement *element = (*elementIt).second;
        if (!element->isLoaded()) element->load();
        return element;
    }

    void Map::load(void)
    {
        _loaded = true;
        std::string elementIdsJson, languagesJson, namesJson;

        mongo::BSONObj projection = BSON("id" << 1);

        auto cursor = _connectionPtr->query("diaphnea.point_elements", MONGO_QUERY("map" << _id), 0, 0, &projection);
        while (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();
            PointElement *element = new PointElement(dbElement.getField("_id").OID(), dbElement.getStringField("id"), _connectionPtr, &_languageIdVector);
            _elementMap.insert(std::pair<std::string, MapElement *>(element->getId(), element));

            if (elementIdsJson.empty()) elementIdsJson = "[\"";
            else elementIdsJson += "\",\"";
            elementIdsJson += element->getId();
        }

        cursor = _connectionPtr->query("diaphnea.polygon_elements", MONGO_QUERY("map" << _id), 0, 0, &projection);
        while (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();
            PolygonElement *element = new PolygonElement(dbElement.getField("_id").OID(), dbElement.getStringField("id"), _connectionPtr, &_languageIdVector);
            _elementMap.insert(std::pair<std::string, MapElement *>(element->getId(), element));

            if (elementIdsJson.empty()) elementIdsJson = "[\"";
            else elementIdsJson += "\",\"";
            elementIdsJson += element->getId();
        }
        if (!elementIdsJson.empty()) elementIdsJson += "\"]";

        cursor = _connectionPtr->query("diaphnea.maps", MONGO_QUERY("_id" << _mongoId), 1);
        if (cursor->more())
        {
            mongo::BSONObj dbMap = cursor->next();

            mongo::BSONObj dbName = dbMap.getField("name").Obj();
            std::vector<mongo::BSONElement> dbLanguageVector = dbMap.getField("languages").Array();
            int i, n = dbLanguageVector.size();
            for (i = 0; i < n; ++i)
            {
                mongo::BSONObj dbLanguage = dbLanguageVector[i].Obj();
                const char *languageId = dbLanguage.getStringField("id");
                const char *languageName = dbLanguage.getStringField("name");
                _languageNameMap.insert(std::pair<std::string, std::string>(languageId, languageName));
                _languageIdVector.push_back(languageId);

                const char *mapName = dbName.getStringField(languageId);
                _nameMap.insert(std::pair<std::string, std::string>(languageId, mapName));

                if (languagesJson.empty()) languagesJson = "[{\"id\":\"";
                else languagesJson += "\"},{\"id\":\"";
                languagesJson += std::string(languageId) + "\",\"name\":\"" + languageName;

                if (namesJson.empty()) namesJson = "{\"";
                else namesJson += "\",\"";
                namesJson += std::string(languageId) + "\":\"" + mapName;
            }
            if (!languagesJson.empty()) languagesJson += "\"}]";
            if (!namesJson.empty()) namesJson += "\"}";

            std::vector<mongo::BSONElement> dbResolutionVector = dbMap.getField("resolutions").Array();
            _sampleLengthVector.resize(dbResolutionVector.size());
            n = dbResolutionVector.size();
            for (i = 0; i < n; ++i)
            {
                mongo::BSONObj dbResolution = dbResolutionVector[i].Obj();
                int index = dbResolution.getIntField("index");
                double sampleLength = dbResolution.getField("sample_length").Double();
                _sampleLengthVector[index] = sampleLength;
            }

            std::vector<mongo::BSONElement> dbLookVector = dbMap.getField("looks").Array();
            n = dbLookVector.size();
            for (i = 0; i < n; ++i)
            {
                mongo::BSONObj dbLook = dbLookVector[i].Obj();
                const char *id = dbLook.getStringField("id");
                const char *type = dbLook.getStringField("type");




            }

            if (elementIdsJson.empty()) elementIdsJson = "undefined";
            if (languagesJson.empty()) languagesJson = "undefined";
            if (namesJson.empty()) namesJson = "undefined";
            _infoJson = "{\"elementIds\":" + elementIdsJson + ",\"languages\":" + languagesJson + ",\"names\":" + namesJson + "}";
        }
    }
}
