#pragma once

#include "MapData.h"

namespace map_server
{
    class ElementName;

    class NameTranslation
    {
    private:
        // Array of ElementNames
        int _nameCount;
        int _names;

    public:
        int getNameCount(void) const { return _nameCount; }
        const ElementName *getElementName(int i) const { return reinterpret_cast<const ElementName *>(elementNames + *(intArrays + _names + i)); }
    };
}
