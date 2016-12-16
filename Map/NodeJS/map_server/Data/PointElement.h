#pragma once

#include "MapElement.h"

namespace map_server
{
    class PointLook;
    class PointItem;

    class PointElement : public MapElement
    {
    private:
        const PointLook *_look;
        PointItem *_item;

        void load(void);

    public:
        PointElement(const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            MapElement(mongoId, id, iMap), _look(0) {}

        const PointLook *getLook(void) const { return _look; }
		PointItem *getItem(void) const { return _item; }
    };
}
