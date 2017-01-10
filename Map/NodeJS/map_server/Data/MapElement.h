#pragma once

#include "IMap.h"

namespace map_server
{
    class MapElement
    {
    protected:
        const mongo::OID _mongoId;
        const std::string _id;
        //int _numericalId;

        IMap * const _iMap;
        bool _loaded;
        std::map<std::string, std::pair<std::string, std::string> > _nameMap;
        std::string _infoJson;

        void loadCommon(mongo::BSONObj dbElement);

    public:
        MapElement(const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            _mongoId(mongoId), _id(id)/*, _numericalId(-1)*/, _iMap(iMap), _loaded(false) {}
        virtual ~MapElement() {}

        bool isLoaded(void) const { return _loaded; }
        virtual void load(void) = 0;

        const std::string& getId(void) const { return _id; }
        const std::string& getInfoJson(void) const { return _infoJson; }

    };
}
