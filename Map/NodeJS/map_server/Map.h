#pragma once

#include "IMap.h"

namespace map_server
{
    class MapElement;
    class MultipointsItem;
    class MapItem;

    class Map : public IMap
    {
    private:
        const mongo::OID _mongoId;
        const std::string _id;

        double _zoomMinDistance;
        double _zoomMaxDistance;
        double _resolutionThreshold;
        double _sizeParameter1;
        double _sizeParameter2;

        mongo::DBClientConnection * const _connectionPtr;
        bool _loaded;
        std::string _emptyString;

        mongo::DBClientConnection *getConnectionPtr(void) { return _connectionPtr; }
        std::vector<std::string> *getLanguageIdVectorPtr(void) { return &_languageIdVector; }
        const Look *getLook(int lookId);
        void addItemLook(const ItemLook *look);

        std::map<std::string, std::string> _languageNameMap;
        std::vector<std::string> _languageIdVector;
        std::map<std::string, std::string> _nameMap;
        std::vector<double> _sampleLengthVector;

        std::map<int, const Look *> _lookMap;
        std::map<int, const ItemLook *> _itemLookMap;

        std::map<std::string, MapElement *> _elementMap;
        std::map<std::string, LineItem *> _lineItemMap;
        std::map<std::string, FilledPolygonItem *> _filledPolygonItemMap;
        std::map<int, MapItem *> _itemMap;

        void addPointLists(MultipointsItem *item, mongo::BSONObj& dbItem);

        std::string _infoJson;

    public:
        Map(const mongo::OID& mongoId, const std::string& id, mongo::DBClientConnection *connectionPtr) :
            _mongoId(mongoId), _id(id), _connectionPtr(connectionPtr), _loaded(false) {}
		~Map();

        const std::string& getId(void) const { return _id; }
        bool isLoaded(void) const { return _loaded; }
        void load(void);
        void addPointItem(PointItem *pointItem);

        double getZoomMinDistance(void) const { return _zoomMinDistance; }
        double getZoomMaxDistance(void) const { return _zoomMaxDistance; }
        double getResolutionThreshold(void) const { return _resolutionThreshold; }
        double getSizeParameter1(void) const { return _sizeParameter1; }
        double getSizeParameter2(void) const { return _sizeParameter2; }

        const std::string& getInfoJson(void) const { return _infoJson; }
        MapElement *getElement(const std::string& id);
        LineItem *getLineItem(const std::string& mongoId);
        FilledPolygonItem *getFilledPolygonItem(const std::string& mongoId);
        MapItem *getItem(int itemId);
        const ItemLook *getItemLook(int lookId);
    };
}
