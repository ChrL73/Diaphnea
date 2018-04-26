#pragma once

#include "MapData.h"

namespace map_server
{
    class MultipointItem;

    class LineElement
    {
    private:
        // String
        int _elementId;

        // Array of MultipointItems
        int _itemCount;
        int _items;

    public:
        static const LineElement *get(int offset) { return reinterpret_cast<const LineElement *>(lineElements + offset); }

        const char *getElementId(void) const { return strings + _elementId; }

        int getItemCount(void) const { return _itemCount; }
        const MultipointItem *getItem(int i) const { return reinterpret_cast<const MultipointItem *>(multipointItems + *(intArrays + _items + i)); }
    };
}
