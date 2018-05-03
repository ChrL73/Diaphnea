#pragma once

#include "MapItem.h"

namespace map_server
{
    class MultipointItem;
    class PolygonElement;
    class PolygonLook;

    class FilledPolygonItem : public MapItem
    {
    private:
        int _fillLookId;
        int _fillZIndex;
        int _fillAlpha;
        int _fillRed;
        int _fillGreen;
        int _fillBlue;
        double _fillSize;

        int getElementLookId(void) const { return _fillLookId; }
        bool hasResolution(void) const { return true; }

    public:
        FilledPolygonItem(const PolygonElement *polygonElement, const MultipointItem *multipointItem, const PolygonLook *polygonLook);

        int getFillLookId(void) const { return _fillLookId; }
        int getFillZIndex(void) const { return _fillZIndex; }
        int getFillAlpha(void) const { return _fillAlpha; }
        int getFillRed(void) const { return _fillRed; }
        int getFillGreen(void) const { return _fillGreen; }
        int getFillBlue(void) const { return _fillBlue; }
    };
}
