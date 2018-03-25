#pragma once

#include "MultipointsItem.h"

namespace map_server
{
    class LineItem : public MultipointsItem
    {
    private:
        const bool _cap1Round;
        const bool _cap2Round;

    public:
        LineItem(int id, int resolutionCount, IMap *iMap, double xMin, double xMax, double yMin, double yMax, bool cap1Round, bool cap2Round) :
            MultipointsItem(id, resolutionCount, iMap, xMin, xMax, yMin, yMax), _cap1Round(cap1Round), _cap2Round(cap2Round) {}

        const std::string& getInfoJson(unsigned int resolutionIndex) const;

        bool cap1Round(void) const { return _cap1Round; }
        bool cap2Round(void) const { return _cap2Round; }
    };
}
