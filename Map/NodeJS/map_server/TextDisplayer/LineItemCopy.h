#pragma once

#include "ItemCopy.h"

#include <set>

namespace map_server
{
    class Point;

    class LineItemCopy : public ItemCopy
    {
    private:
        std::vector<std::vector<const Point *> > _pointVector;

        std::set<double> **_hIntersections;
        int _height;
        double _yMin;
        double _yMax;

        std::set<double> **_vIntersections;
        int _width;
        double _xMin;
        double _xMax;

    public:
        LineItemCopy(const std::string& elementId, double importance) : ItemCopy(elementId, importance) {}

        void addPoint(double x, double y, bool newLine);
        void setIntersections(double height, double width);

        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }
        std::set<double> *getHIntersections(int y);

        double getXMin(void) const { return _xMin; }
        double getXMax(void) const { return _xMax; }
        std::set<double> *getVIntersections(int x);
    };
}
