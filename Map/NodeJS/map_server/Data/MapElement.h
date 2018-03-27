#pragma once

#include "IMap.h"

namespace map_server
{
    class ElementName;
    class Category;

    class MapElement
    {
    private:
        void addNames(const std::string& name, std::vector<ElementName *>& nameVector);
        virtual bool allowMultiline(void) = 0;
        const Category *_category;

    protected:
        static constexpr int _maxIntDbValue = 0xfffff;

        const std::string _dbName;
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
        MapElement(const std::string& dbName, const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            _category(0), _dbName(dbName), _mongoId(mongoId), _id(id), _importance(0.0), _iMap(iMap), _loaded(false), _error(false) {}
        virtual ~MapElement();

        bool isLoaded(void) const { return _loaded; }
        bool error(void) const { return _error; }
        virtual void load(void) = 0;

        const Category *getCategory(void) const { return _category; }
        const std::string& getId(void) const { return _id; }
        const std::string& getInfoJson(void) const { return _infoJson; }

    };
}
