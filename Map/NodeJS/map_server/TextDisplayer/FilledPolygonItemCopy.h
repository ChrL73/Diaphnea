#pragma once

#include "ItemCopy.h"

namespace map_server
{
    class FilledPolygonItemCopy : public ItemCopy
    {
    public:
        FilledPolygonItemCopy(const std::string& elementId) : ItemCopy(elementId) {}
    };
}
