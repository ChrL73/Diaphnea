#pragma once

#include "MapData.h"

namespace map_server
{
    class LineElement
    {
    private:
        // String
        int _elementId;

    public:
        static const LineElement *get(int offset) { return reinterpret_cast<const LineElement *>(lineElements + offset); }

        const char *getElementId(void) const { return strings + _elementId; }

    };
}
