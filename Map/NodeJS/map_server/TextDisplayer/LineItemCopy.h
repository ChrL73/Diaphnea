#pragma once

#include "ItemCopy.h"

namespace map_server
{
    class LineItemCopy : public ItemCopy
    {
    public:
        LineItemCopy(int elementId) : ItemCopy(elementId) {}
    };
}
