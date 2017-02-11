#pragma once

#include "MultipointsItem.h"

namespace map_server
{
    class LineItem : public MultipointsItem
    {
    private:
        const bool _cap1Round;
        const bool _cap2Round;

        void setInfoJsonVector(void);

    public:
        LineItem(int id, int resolutionCount, bool cap1Round, bool cap2Round) :
            MultipointsItem(id, resolutionCount), _cap1Round(cap1Round), _cap2Round(cap2Round) {}

        bool cap1Round(void) const { return _cap1Round; }
        bool cap2Round(void) const { return _cap2Round; }
    };
}
