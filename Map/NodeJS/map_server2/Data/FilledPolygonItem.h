#pragma once

#include "MapItem.h"

namespace map_server
{
    class MultipointItem;
    class PolygonElement;
    class PolygonLook;
    class PointList;
    class Point;

    class FilledPolygonItem : public MapItem
    {
    private:
        int _fillLookId;
        int _fillZIndex;
        int _fillAlpha;
        int _fillRed;
        int _fillGreen;
        int _fillBlue;

        const PointList * const _pointList;
        std::vector<const Point *> _pointVector;

        int getElementLookId(void) const { return _fillLookId; }
        bool hasResolution(void) const { return true; }

    public:
        FilledPolygonItem(const PolygonElement *polygonElement, const NameTranslation *name, const MultipointItem *multipointItem, const PolygonLook *polygonLook, int resolutionIndex);

        int getFillLookId(void) const { return _fillLookId; }
        int getFillZIndex(void) const { return _fillZIndex; }
        int getFillAlpha(void) const { return _fillAlpha; }
        int getFillRed(void) const { return _fillRed; }
        int getFillGreen(void) const { return _fillGreen; }
        int getFillBlue(void) const { return _fillBlue; }

        const PointList *getPointList(void) const { return _pointList; }
        void addPoint(double x, double y);
    };
}
