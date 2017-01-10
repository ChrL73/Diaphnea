#pragma once

#include "ItemCopy.h"

namespace map_server
{
    class PointItemCopy : public ItemCopy
    {
    private:
        const double _x;
        const double _y;
        const double _diameter;

    public:
        PointItemCopy(const std::string& elementId, double x, double y, double diameter) :
            ItemCopy(elementId), _x(x), _y(y), _diameter(diameter) {}

        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
        double getDiameter(void) const { return _diameter; }
    };
}
