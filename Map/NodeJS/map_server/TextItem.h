#pragma once

#include "MapItem.h"

namespace map_server
{
    class TextItem : public MapItem
    {
    private:
        bool hasResolution(void) const { return false; }
    };
}
