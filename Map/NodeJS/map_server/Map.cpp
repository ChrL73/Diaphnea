#include "Map.h"
#include "PointElement.h"
#include "PolygonElement.h"
#include "LineElement.h"
#include "PointLook.h"
#include "LineLook.h"
#include "PolygonLook.h"
#include "LineItem.h"
#include "FilledPolygonItem.h"
#include "PointItem.h"
#include "Point.h"
#include "ItemLook.h"

namespace map_server
{
    Map::~Map()
    {
        std::map<std::string, MapElement *>::iterator elementIt = _elementMap.begin();
        for (; elementIt != _elementMap.end(); ++elementIt) delete (*elementIt).second;

        std::map<int, MapItem *>::iterator itemIt = _itemMap.begin();
        for (; itemIt != _itemMap.end(); ++itemIt) delete (*itemIt).second;

        std::map<int, const Look *>::iterator lookIt = _lookMap.begin();
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

    LineItem *Map::getLineItem(const std::string& mongoId)
    {
        std::map<std::string, LineItem *>::iterator itemIt = _lineItemMap.find(mongoId);

        if (itemIt == _lineItemMap.end())
        {
            auto cursor = _connectionPtr->query("diaphnea.items", MONGO_QUERY("_id" << mongo::OID(mongoId)), 1);
            if (cursor->more())
            {
                mongo::BSONObj dbItem = cursor->next();

                int itemId = dbItem.getIntField("item_id");
                LineItem *item = new LineItem(itemId, _sampleLengthVector.size());
                addPointLists(item, dbItem);

                itemIt = _lineItemMap.insert(std::pair<std::string, LineItem *>(mongoId, item)).first;
                _itemMap.insert(std::pair<int, MapItem *>(itemId, item));
            }
            else
            {
                itemIt = _lineItemMap.insert(std::pair<std::string, LineItem *>(mongoId, 0)).first;
            }
        }

        return (*itemIt).second;
    }

    FilledPolygonItem *Map::getFilledPolygonItem(const std::string& mongoId)
    {
        std::map<std::string, FilledPolygonItem *>::iterator itemIt = _filledPolygonItemMap.find(mongoId);

        if (itemIt == _filledPolygonItemMap.end())
        {
            auto cursor = _connectionPtr->query("diaphnea.items", MONGO_QUERY("_id" << mongo::OID(mongoId)), 1);
            if (cursor->more())
            {
                mongo::BSONObj dbItem = cursor->next();

                int itemId = dbItem.getIntField("item_id");
                FilledPolygonItem *item = new FilledPolygonItem(itemId, _sampleLengthVector.size());
                addPointLists(item, dbItem);

                itemIt = _filledPolygonItemMap.insert(std::pair<std::string, FilledPolygonItem *>(mongoId, item)).first;
                _itemMap.insert(std::pair<int, MapItem *>(itemId, item));
            }
            else
            {
                itemIt = _filledPolygonItemMap.insert(std::pair<std::string, FilledPolygonItem *>(mongoId, 0)).first;
            }
        }

        return (*itemIt).second;
    }

    void Map::addPointLists(MultipointsItem *item, mongo::BSONObj& dbItem)
    {
        std::vector<mongo::BSONElement> pointListIdVector = dbItem.getField("point_lists").Array();
        int i, n = pointListIdVector.size();
        for (i = 0; i < n; ++i)
        {
            auto cursor = _connectionPtr->query("diaphnea.point_lists", MONGO_QUERY("_id" << pointListIdVector[i].OID()), 1);
            if (cursor->more())
            {
                mongo::BSONObj pointList = cursor->next();
                std::vector<mongo::BSONElement> pointVector = pointList.getField("points").Array();
                int j, m = pointVector.size();
                for (j = 0; j < m; ++j)
                {
                    mongo::BSONObj dbPoint = pointVector[j].Obj();
                    double x = dbPoint.getField("x").Double();
                    double y = dbPoint.getField("y").Double();
                    Point *point = new Point(x, -y);
                    item->addPoint(i, _sampleLengthVector[i], point);
                }
            }
        }

        item->setInfoJsonVector();
    }

    void Map::addPointItem(PointItem *pointItem)
    {
        _itemMap.insert(std::pair<int, MapItem *>(pointItem->getId(), pointItem));
    }

    MapItem *Map::getItem(int itemId)
    {
        std::map<int, MapItem *>::iterator it = _itemMap.find(itemId);
        if (it != _itemMap.end()) return (*it).second;
        return 0;
    }

    const Look *Map::getLook(int lookId)
    {
        std::map<int, const Look *>::iterator it = _lookMap.find(lookId);
        if (it != _lookMap.end()) return (*it).second;
        return 0;
    }

    void Map::addItemLook(const ItemLook *look)
    {
        _itemLookMap.insert(std::pair<int, const ItemLook *>(look->getId(), look));
    }

    const ItemLook *Map::getItemLook(int lookId)
    {
        std::map<int, const ItemLook *>::iterator it = _itemLookMap.find(lookId);
        if (it != _itemLookMap.end()) return (*it).second;
        return 0;
    }

    int Map::getResolutionIndex(double scale)
    {
        double wantedLength = _resolutionThreshold / scale;
        int i, n = _sampleLengthVector.size();
        for (i = n - 1; i > 0; --i)
        {
            if (wantedLength > _sampleLengthVector[i]) break;
        }

        return i;
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

        cursor = _connectionPtr->query("diaphnea.line_elements", MONGO_QUERY("map" << _id), 0, 0, &projection);
        while (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();
            LineElement *element = new LineElement(dbElement.getField("_id").OID(), dbElement.getStringField("id"), this);
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

            _zoomMinDistance = dbMap.getField("zoom_min_distance").Double();
            _zoomMaxDistance = dbMap.getField("zoom_max_distance").Double();
            _resolutionThreshold = dbMap.getField("resolution_threshold").Double();
            _sizeParameter1 = dbMap.getField("size_parameter1").Double();
            _sizeParameter2 = dbMap.getField("size_parameter2").Double();

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
                int id = dbLook.getIntField("id");
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
                                                    pointRed, pointGreen, pointBlue, pointSize, this);
                    _lookMap.insert(std::pair<int, const Look *> (id, look));
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
                                                   lineRed, lineGreen, lineBlue, lineSize, this);
                    _lookMap.insert(std::pair<int, const Look *> (id, look));
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
                                                               contourRed, contourGreen, contourBlue, contourSize, fillLightRatio, fillZIndex, this);
                    _lookMap.insert(std::pair<int, const Look *> (id, look));
                  }
            }

            if (elementIdsJson.empty()) elementIdsJson = "undefined";
            if (languagesJson.empty()) languagesJson = "undefined";
            if (namesJson.empty()) namesJson = "undefined";

            std::stringstream jsonStream;
            jsonStream << "{\"elementIds\":" << elementIdsJson << ",\"languages\":" << languagesJson << ",\"names\":" << namesJson
                       << ",\"zoomMinDistance\":" << _zoomMinDistance << ",\"zoomMaxDistance\":" << _zoomMaxDistance
                       << ",\"sizeParameter1\":" << _sizeParameter1 << ",\"sizeParameter2\":" << _sizeParameter2 << "}";
            _infoJson = jsonStream.str();
        }
    }
}
