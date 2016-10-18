#pragma once

#include "MapElement.h"

namespace map_server
{
    class PolygonElement : public MapElement
    {
    private:
        void load(void);

    public:
        PolygonElement(const mongo::OID& mongoId, const std::string& id) :
            MapElement(mongoId, id) {}
    };
}

