#pragma once

#include "MultipointsItem.h"

namespace map_server
{
    class LineItem : public MultipointsItem
    {
    private:
        void setInfoJsonVector(void);

    public:
        LineItem(int id, int resolutionCount) : MultipointsItem(id, resolutionCount) {}
    };
}
