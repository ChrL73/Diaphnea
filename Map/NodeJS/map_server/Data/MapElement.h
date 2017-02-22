#pragma once

#include "IMap.h"

namespace map_server
{
    class ElementName;

    class MapElement
    {
    private:
        void addNames(const std::string& name, std::vector<ElementName *>& nameVector);

        virtual bool allowMultiline(void) = 0;

    protected:
        const mongo::OID _mongoId;
        const std::string _id;
        double _importance;

        IMap * const _iMap;
        bool _loaded;
		bool _error;
        std::map<std::string, std::vector<ElementName *> > _nameMap;
        std::string _infoJson;

        bool loadCommon(mongo::BSONObj dbElement);

    public:
        MapElement(const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            _mongoId(mongoId), _id(id), _iMap(iMap), _loaded(false), _error(false) {}
        virtual ~MapElement();

        bool isLoaded(void) const { return _loaded; }
		bool error(void) const { return _error; }
        virtual void load(void) = 0;

        const std::string& getId(void) const { return _id; }
        const std::string& getInfoJson(void) const { return _infoJson; }

    };
}
