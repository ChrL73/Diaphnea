#pragma once

#include "MapData.h"

namespace map_server
{
    class PolygonElement
    {
    private:
        // String
        int _elementId;

    public:
        static const PolygonElement *get(int offset) { return reinterpret_cast<const PolygonElement *>(polygonElements + offset); }

        const char *getElementId(void) const { return strings + _elementId; }

    };
}
