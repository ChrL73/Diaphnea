#pragma once

#include "MapElement.h"

namespace map_server
{
    class PolygonLook;

    class PolygonElement : public MapElement
    {
    private:
        const PolygonLook *_look;

        void load(void);

    public:
        PolygonElement(const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            MapElement(mongoId, id, iMap), _look(0) {}
    };
}

