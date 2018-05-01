#pragma once

#include "MapData.h"

namespace map_server
{
    class MultipointItem;
    class LineLook;

    class LineElement
    {
    private:
        // String
        int _elementId;

        // Array of MultipointItems
        int _itemCount;
        int _items;

        // Array of LineLooks
        int _lookCount;
        int _looks;

        // Int
        int _framingLevel;

    public:
        static const LineElement *get(int offset) { return reinterpret_cast<const LineElement *>(lineElements + offset); }

        const char *getElementId(void) const { return strings + _elementId; }

        int getItemCount(void) const { return _itemCount; }
        const MultipointItem *getItem(int i) const { return reinterpret_cast<const MultipointItem *>(multipointItems + *(intArrays + _items + i)); }

        int getLookCount(void) const { return _lookCount; }
        const LineLook *getLook(int i) const;

        int getFramingLevel(void) const { return _framingLevel; }
    };
}
