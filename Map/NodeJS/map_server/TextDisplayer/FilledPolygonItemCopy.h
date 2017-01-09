#pragma once

#include "ItemCopy.h"

namespace map_server
{
    class FilledPolygonItemCopy : public ItemCopy
    {
    public:
        FilledPolygonItemCopy(int elementId) : ItemCopy(elementId) {}
    };
}
