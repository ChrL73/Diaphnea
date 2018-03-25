#pragma once

#include "MultipointsItem.h"

namespace map_server
{
    class FilledPolygonItem : public MultipointsItem
    {
    public:
        FilledPolygonItem(int id, int resolutionCount, IMap *iMap, double xMin, double xMax, double yMin, double yMax) :
            MultipointsItem(id, resolutionCount, iMap, xMin, xMax, yMin, yMax) {}

        const std::string& getInfoJson(unsigned int resolutionIndex) const;
    };
}
