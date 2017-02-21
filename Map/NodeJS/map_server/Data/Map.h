#pragma once

#include "IMap.h"

#include <set>

namespace map_server
{
    class MapElement;
    class MultipointsItem;
    class MapItem;
	class DatabaseError;

    class Map : public IMap
    {
    private:
		static constexpr int _maxIntDbValue = 0xfffff;

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
        const Look *getLook(int lookId) const;
        void addItemLook(const ItemLook *look);

        std::map<std::string, std::string> _languageNameMap;
        std::vector<std::string> _languageIdVector;
        std::set<std::string> _languageIdSet;
        std::map<std::string, std::string> _nameMap;
        std::vector<double> _sampleLengthVector;

        std::map<int, const Look *> _lookMap;
        std::map<int, const ItemLook *> _itemLookMap;

        std::map<int, std::string> _itemToElement0Map;
        bool _itemToElement0MapLoaded;
        void loadItemToElement0Map(void);

        std::map<std::string, MapElement *> _elementMap;
        std::map<std::string, LineItem *> _lineItemMap;
        std::map<std::string, FilledPolygonItem *> _filledPolygonItemMap;
        std::map<int, MapItem *> _itemMap;

        bool addPointLists(MultipointsItem *item, mongo::BSONObj& dbItem);

        std::string _infoJson;

		std::vector<DatabaseError *> _errorVector;

    public:
        Map(const mongo::OID& mongoId, const std::string& id, mongo::DBClientConnection *connectionPtr) :
            _mongoId(mongoId), _id(id), _connectionPtr(connectionPtr), _loaded(false), _itemToElement0MapLoaded(false) {}
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
        LineItem *getLineItem(const mongo::OID& mongoId);
        FilledPolygonItem *getFilledPolygonItem(const mongo::OID& mongoId);
        MapItem *getItem(int itemId);
        const ItemLook *getItemLook(int lookId) const;
        int getResolutionIndex(double scale);
        bool knownLanguage(const std::string& languageId) const { return _languageIdSet.find(languageId) != _languageIdSet.end(); }
		std::vector<DatabaseError *>& getErrorVector(void) { return _errorVector; }
    };
}
