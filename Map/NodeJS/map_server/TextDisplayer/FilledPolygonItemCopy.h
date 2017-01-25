#pragma once

#include "ItemCopy.h"

#include <set>

namespace map_server
{
    class Point;

    class FilledPolygonItemCopy : public ItemCopy
    {
    private:
        std::vector<const Point *> _pointVector;
        std::set<double> **_intersections;
        int _height;
        double _yMin;
        double _yMax;

    public:
        FilledPolygonItemCopy(const std::string& elementId, double importance) :
            ItemCopy(elementId, importance), _intersections(0), _height(0), _yMin(0.0), _yMax(0.0) {}
        ~FilledPolygonItemCopy();

        void addPoint(double x, double y);
        void setIntersections(double height, double width);

        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }
        std::set<double> *getIntersections(int y);
    };
}
