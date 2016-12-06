#pragma once

#include "MapItem.h"

namespace map_server
{
    class Point;

    class PointItem : public MapItem
    {
    private:
        Point *_point;
        bool hasResolution(void) const { return false; }

    public:
        PointItem(int id, Point *point);
		~PointItem();

    };
}
