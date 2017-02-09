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
        PointElement(const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            MapElement(mongoId, id, iMap) {}

        const PointLook *getLook(int i) const { return _lookVector[i]; }
		PointItem *getItem(void) const { return _item; }
    };
}
