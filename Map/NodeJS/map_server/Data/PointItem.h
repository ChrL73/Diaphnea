#pragma once

#include "MapItem.h"

namespace map_server
{
    class Point;

    class PointItem : public MapItem
    {
    private:
        const Point *_point;
        bool hasResolution(void) const { return false; }

    public:
        PointItem(int id, const Point *point);
        ~PointItem();

        const Point *getPoint(void) const { return _point; }
    };
}
