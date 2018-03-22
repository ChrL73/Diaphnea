#pragma once

#include "MapElement.h"

namespace map_server
{
    class LineLook;

    class LineElement : public MapElement
    {
    private:
        std::vector<const LineLook *> _lookVector;
        std::vector<LineItem *> _itemVector;

        void load(void);
        bool allowMultiline(void) { return false; }

    public:
        LineElement(const std::string& dbName, const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            MapElement(dbName, mongoId, id, iMap) {}

        const std::vector<LineItem *>& getItemVector(void) const { return _itemVector; }
    };
}

