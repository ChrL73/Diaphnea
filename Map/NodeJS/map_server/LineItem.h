#pragma once

#include "MultipointsItem.h"

namespace map_server
{
    class LineItem : public MultipointsItem
    {
    private:


    public:
        LineItem(int id) : MultipointsItem(id) {}
    };
}
