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
        std::map<std::string, MapElement *>::iterator elementIt = _elementMap.begin();
        for (; elementIt != _elementMap.end(); ++elementIt) delete (*elementIt).second;

        std::map<std::string, const Look *>::iterator lookIt = _lookMap.begin();
        for (; lookIt != _lookMap.end(); ++lookIt) delete (*lookIt).second;
    }

    MapElement *Map::getElement(const std::string& id)
    {
        std::map<std::string, MapElement *>::iterator elementIt = _elementMap.find(id);
        if (elementIt == _elementMap.end()) return 0;

        MapElement *element = (*elementIt).second;
        if (!element->isLoaded()) element->load();
        return element;
    }

    const Look *Map::getLook(const std::string& lookId)
    {
        std::map<std::string, const Look *>::iterator it = _lookMap.find(lookId);
        if (it != _lookMap.end()) return (*it).second;
        return 0;
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
            PointElement *element = new PointElement(dbElement.getField("_id").OID(), dbElement.getStringField("id"), this);
            _elementMap.insert(std::pair<std::string, MapElement *>(element->getId(), element));

            if (elementIdsJson.empty()) elementIdsJson = "[\"";
            else elementIdsJson += "\",\"";
            elementIdsJson += element->getId();
        }

        cursor = _connectionPtr->query("diaphnea.polygon_elements", MONGO_QUERY("map" << _id), 0, 0, &projection);
        while (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();
            PolygonElement *element = new PolygonElement(dbElement.getField("_id").OID(), dbElement.getStringField("id"), this);
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
                int textAlpha = dbLook.getIntField("text_alpha");
                int textRed = dbLook.getIntField("text_red");
                int textGreen = dbLook.getIntField("text_green");
                int textBlue = dbLook.getIntField("text_blue");
                double textSize = dbLook.getField("text_size").Double();

                if (strcmp(type, "point") == 0)
                {
                    int pointZIndex = dbLook.getIntField("point_z_index");
                    int pointAlpha = dbLook.getIntField("point_alpha");
                    int pointRed = dbLook.getIntField("point_red");
                    int pointGreen = dbLook.getIntField("point_green");
                    int pointBlue = dbLook.getIntField("point_blue");
                    double pointSize = dbLook.getField("point_size").Double();

                    PointLook *look = new PointLook(id, textAlpha, textRed, textGreen, textBlue, textSize, pointZIndex, pointAlpha,
                                                    pointRed, pointGreen, pointBlue, pointSize);
                    _lookMap.insert(std::pair<std::string, const Look *> (id, look));
                }
                else if (strcmp(type, "line") == 0)
                {
                    int lineZIndex = dbLook.getIntField("line_z_index");
                    int lineAlpha = dbLook.getIntField("line_alpha");
                    int lineRed = dbLook.getIntField("line_red");
                    int lineGreen = dbLook.getIntField("line_green");
                    int lineBlue = dbLook.getIntField("line_blue");
                    double lineSize = dbLook.getField("line_size").Double();

                    LineLook *look = new LineLook(id, textAlpha, textRed, textGreen, textBlue, textSize, lineZIndex, lineAlpha,
                                                   lineRed, lineGreen, lineBlue, lineSize);
                    _lookMap.insert(std::pair<std::string, const Look *> (id, look));
                }
                else if (strcmp(type, "polygon") == 0)
                {
                    int contourZIndex = dbLook.getIntField("contour_z_index");
                    int contourAlpha = dbLook.getIntField("contour_alpha");
                    int contourRed = dbLook.getIntField("contour_red");
                    int contourGreen = dbLook.getIntField("contour_green");
                    int contourBlue = dbLook.getIntField("contour_blue");
                    double contourSize = dbLook.getField("contour_size").Double();
                    double fillLightRatio = dbLook.getField("fill_light_ratio").Double();
                    int fillZIndex = dbLook.getIntField("fill_z_index");

                    PolygonLook *look = new PolygonLook(id, textAlpha, textRed, textGreen, textBlue, textSize, contourZIndex, contourAlpha,
                                                               contourRed, contourGreen, contourBlue, contourSize, fillLightRatio, fillZIndex);
                    _lookMap.insert(std::pair<std::string, const Look *> (id, look));
                  }
            }

            if (elementIdsJson.empty()) elementIdsJson = "undefined";
            if (languagesJson.empty()) languagesJson = "undefined";
            if (namesJson.empty()) namesJson = "undefined";
            _infoJson = "{\"elementIds\":" + elementIdsJson + ",\"languages\":" + languagesJson + ",\"names\":" + namesJson + "}";
        }
    }
}
