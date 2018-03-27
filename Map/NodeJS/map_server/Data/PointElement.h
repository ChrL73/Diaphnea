#pragma once

#include "MapElement.h"

#include <vector>

namespace map_server
{
    class PointLook;
    class PointItem;

    class PointElement : public MapElement
    {
    private:
        std::vector<const PointLook *> _lookVector;
        PointItem *_item;

        void load(void);
        bool allowMultiline(void) { return false; }

    public:
        PointElement(const std::string& dbName, const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            MapElement(dbName, mongoId, id, iMap) {}

        const PointLook *getLook(int i) const;
        PointItem *getItem(void) const { return _item; }
    };
}
