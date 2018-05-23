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
        const char * const _pointColor;
        const double _pointOpacity;
        const double _pointSize;

        double _x;
        double _y;
        double _diameter;

        int getElementLookId(void) const { return _pointLookId; }
        bool hasResolution(void) const { return false; }
        int getZIndex(void) const { return _pointZIndex; }

    public:
        PointItem(const PointElement *pointElement, const NameTranslation *name, const PointLook *pointLook);

        int getPointLookId(void) const { return _pointLookId; }
        int getPointZIndex(void) const { return _pointZIndex; }
        const char *getPointColor(void) const { return _pointColor; }
        double getPointOpacity(void) const { return _pointOpacity; }
        double getPointSize(void) const { return _pointSize; }

        void setX(double x) { _x = x; }
        void setY(double y) { _y = y; }
        void setDiameter(double diameter) { _diameter = diameter; }

        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
        double getDiameter(void) const { return _diameter; }
    };
}

