#include "Map.h"
#include "PointElement.h"
#include "PolygonElement.h"
#include "LineElement.h"

namespace map_server
{
	Map::~Map()
	{
		std::map<std::string, MapElement *>::iterator it = _elementMap.begin();
		for (; it != _elementMap.end(); ++it) delete (*it).second;
	}

    void Map::load(void)
    {
        _loaded = true;

        mongo::BSONObj projection = BSON("id" << 1);

        auto cursor = _connectionPtr->query("diaphnea.point_elements", MONGO_QUERY("map" << _id), 0, 0, &projection);
        while (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();
            PointElement *element = new PointElement(dbElement.getField("_id").OID(), dbElement.getStringField("id"));
            _elementMap.insert(std::pair<std::string, MapElement *>(element->getId(), element));

            if (_elementIdsJson.empty()) _elementIdsJson = "[\"";
            else _elementIdsJson += "\",\"";
            _elementIdsJson += element->getId();
        }

        cursor = _connectionPtr->query("diaphnea.polygon_elements", MONGO_QUERY("map" << _id), 0, 0, &projection);
        while (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();
            PolygonElement *element = new PolygonElement(dbElement.getField("_id").OID(), dbElement.getStringField("id"));
            _elementMap.insert(std::pair<std::string, MapElement *>(element->getId(), element));

            if (_elementIdsJson.empty()) _elementIdsJson = "[\"";
            else _elementIdsJson += "\",\"";
            _elementIdsJson += element->getId();
        }
        if (!_elementIdsJson.empty()) _elementIdsJson += "\"]";

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

                const char *mapName = dbName.getStringField(languageId);
                _nameMap.insert(std::pair<std::string, std::string>(languageId, mapName));

                if (_languagesJson.empty()) _languagesJson = "[{\"id\":\"";
                else _languagesJson += "\"},{\"id\":\"";
                _languagesJson += std::string(languageId) + "\",\"name\":\"" + languageName;
            }
            if (!_languagesJson.empty()) _languagesJson += "\"}]";

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
        }
    }

    const std::string& Map::getName(const std::string& languageId) const
    {
        std::map<std::string, std::string>::const_iterator it = _nameMap.find(languageId);
        if (it != _nameMap.end()) return (*it).second;
        return _emptyString;
    }
}
