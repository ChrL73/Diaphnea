#pragma once

#include "MultipointsItem.h"

namespace map_server
{
    class FilledPolygonItem : public MultipointsItem
    {
    public:
        FilledPolygonItem(int id, int resolutionCount, IMap *iMap) : MultipointsItem(id, resolutionCount, iMap) {}

        const std::string& getInfoJson(unsigned int resolutionIndex) const;
    };
}
