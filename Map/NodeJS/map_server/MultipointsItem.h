#pragma once

#include "MapItem.h"

#include <vector>
#include <map>

namespace map_server
{
    class Point;

    class MultipointsItem : public MapItem
    {
    private:
        std::map<double, std::vector<Point *> > _pointVectorMap;

    protected:
        MultipointsItem(int id) : MapItem(id) {}

    public:

        void addPoint(double samplingLength, Point *point);
    };
}
