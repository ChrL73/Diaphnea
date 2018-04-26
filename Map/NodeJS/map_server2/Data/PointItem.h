#pragma once

#include <string>

namespace map_server
{
    class PointElement;

    class PointItem
    {
    private:
        const int _itemId;
        const std::string _elementId;
        const double _x;
        const double _y;

    public:
        PointItem(const PointElement *pointElement);

        int getItemId(void) const { return _itemId; }
        const std::string& getElementId(void) const { return _elementId; }
        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
    };
}

