#pragma once

#include "ItemCopy.h"

#include <map>

namespace map_server
{
    class Point;

    class LineItemCopy : public ItemCopy
    {
    private:
        std::vector<std::vector<const Point *> > _pointVector;

        std::map<double, double> **_hIntersections;
        int _height;
        double _yMin;
        double _yMax;

        std::map<double, double> **_vIntersections;
        int _width;
        double _xMin;
        double _xMax;

    public:
        LineItemCopy(const std::string& elementId, double importance) :
            ItemCopy(elementId, importance), _hIntersections(0), _height(0), _yMin(0.0), _yMax(0.0), _vIntersections(0), _width(0), _xMin(0.0), _xMax(0.0) {}
        ~LineItemCopy();

        void addPoint(double x, double y, bool newLine);
        void setIntersections(double height, double width);

        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }
        std::map<double, double> *getHIntersections(int y);

        double getXMin(void) const { return _xMin; }
        double getXMax(void) const { return _xMax; }
        std::map<double, double> *getVIntersections(int x);
    };
}
