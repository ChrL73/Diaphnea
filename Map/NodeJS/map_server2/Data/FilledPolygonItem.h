#pragma once

#include "MapItem.h"

#include <set>

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

        const MultipointItem * const _multipointItem;
        std::vector<const Point *> _pointVector;
        std::set<double> **_intersections;
        int _height;
        double _yMinP;
        double _yMaxP;

        int getElementLookId(void) const { return _fillLookId; }
        bool hasResolution(void) const { return true; }

    public:
        FilledPolygonItem(const PolygonElement *polygonElement, const NameTranslation *name, const MultipointItem *multipointItem, const PolygonLook *polygonLook);

        int getFillLookId(void) const { return _fillLookId; }
        int getFillZIndex(void) const { return _fillZIndex; }
        int getFillAlpha(void) const { return _fillAlpha; }
        int getFillRed(void) const { return _fillRed; }
        int getFillGreen(void) const { return _fillGreen; }
        int getFillBlue(void) const { return _fillBlue; }

        const PointList *getPointList(int resolutionIndex) const;
        void addPoint(double x, double y);
        void setIntersections(double height, double width);

        double getYMinP(void) const { return _yMinP; }
        double getYMaxP(void) const { return _yMaxP; }
        std::set<double> *getIntersections(int y);
    };
}
