#pragma once

#include "MapItem.h"

namespace map_server
{
    class Point;

    class PointItem : public MapItem
    {
    private:
        Point *_point;

    public:
        PointItem(int id, Point *point) : MapItem(id), _point(point) {}
		~PointItem();

    };
}
