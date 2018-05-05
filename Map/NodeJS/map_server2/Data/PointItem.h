#pragma once

#include "MapItem.h"

namespace map_server
{
    class PointElement;
    class PointLook;

    class PointItem : public MapItem
    {
    private:
        const int _pointLookId;
        const int _pointZIndex;
        const int _pointAlpha;
        const int _pointRed;
        const int _pointGreen;
        const int _pointBlue;
        const double _pointSize;

        double _x;
        double _y;
        double _diameter;

        int getElementLookId(void) const { return _pointLookId; }
        bool hasResolution(void) const { return false; }

    public:
        PointItem(const PointElement *pointElement, const PointLook *pointLook);

        int getPointLookId(void) const { return _pointLookId; }
        int getPointZIndex(void) const { return _pointZIndex; }
        int getPointAlpha(void) const { return _pointAlpha; }
        int getPointRed(void) const { return _pointRed; }
        int getPointGreen(void) const { return _pointGreen; }
        int getPointBlue(void) const { return _pointBlue; }
        double getPointSize(void) const { return _pointSize; }

        void setX(double x) { _x = x; }
        void setY(double y) { _y = y; }
        void setDiameter(double diameter) { _diameter = diameter; }

        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
        double getDiameter(void) const { return _diameter; }
    };
}

