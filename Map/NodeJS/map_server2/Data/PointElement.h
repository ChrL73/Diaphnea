#pragma once

#include "MapData.h"

namespace map_server
{
    class PointLook;

    class PointElement
    {
    private:
        // String
        int _elementId;

        // Int
        int _itemId;

        // Double
        double _x;

        // Double
        double _y;

        // Array of PointLooks
        int _lookCount;
        int _looks;

    public:
        static const PointElement *get(int offset) { return reinterpret_cast<const PointElement *>(pointElements + offset); }

        const char *getElementId(void) const { return strings + _elementId; }
        int getItemId(void) const { return _itemId; }
        double getX(void) const { return _x; }
        double getY(void) const { return _y; }

        int getLookCount(void) const { return _lookCount; }
        const PointLook *getLook(int i) const { return reinterpret_cast<const PointLook *>(pointLooks + *(intArrays + _looks + i)); }

    };
}

