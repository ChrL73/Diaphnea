#pragma once

#include "MultipointsItem.h"

namespace map_server
{
    class FilledPolygonItem : public MultipointsItem
    {
    private:
        void setInfoJsonVector(void);

    public:
        FilledPolygonItem(int id, int resolutionCount) : MultipointsItem(id, resolutionCount) {}
    };
}
