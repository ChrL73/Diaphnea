#pragma once

#include "MapElement.h"

namespace map_server
{
    class LineLook;

    class LineElement : public MapElement
    {
    private:
        const LineLook *_look;
        std::vector<LineItem *> _itemVector;

        void load(void);
        bool allowMultiline(void) { return false; }

    public:
        LineElement(const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            MapElement(mongoId, id, iMap), _look(0) {}

        const std::vector<LineItem *>& getItemVector(void) const { return _itemVector; }
    };
}

