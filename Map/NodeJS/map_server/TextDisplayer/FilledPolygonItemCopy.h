#pragma once

#include "ItemCopy.h"

#include <vector>
#include <set>

namespace map_server
{
    class Point;

    class FilledPolygonItemCopy : public ItemCopy
    {
    private:
        std::vector<const Point *> _pointVector;
        std::multiset<double> **_intersections;
        int _height;
        double _yMin;
        double _yMax;

    public:
        FilledPolygonItemCopy(const std::string& elementId) :
            ItemCopy(elementId), _intersections(0), _height(0), _yMin(0.0), _yMax(0.0) {}
        ~FilledPolygonItemCopy();

        void addPoint(double x, double y);
        void setIntersections(double height);
    };
}
