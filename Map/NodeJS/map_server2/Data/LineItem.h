#pragma once

#include "MapItem.h"

namespace map_server
{
    class MultipointItem;
    class LineLook;
    class PolygonLook;
    class LineElement;

    class LineItem : public MapItem
    {
    private:
        int _lineLookId;
        int _lineZIndex;
        int _lineAlpha;
        int _lineRed;
        int _lineGreen;
        int _lineBlue;
        double _lineSize;

        int getElementLookId(void) const { return _lineLookId; }
        bool hasResolution(void) const { return true; }

    public:
        LineItem(const LineElement *lineElement, const MultipointItem *multipointItem, const LineLook *lineLook);
        LineItem(const MultipointItem *multipointItem, const PolygonLook *polygonLook);

        void updateLook(const PolygonLook *polygonLook);

        int getLineLookId(void) const { return _lineLookId; }
        int getLineZIndex(void) const { return _lineZIndex; }
        int getLineAlpha(void) const { return _lineAlpha; }
        int getLineRed(void) const { return _lineRed; }
        int getLineGreen(void) const { return _lineGreen; }
        int getLineBlue(void) const { return _lineBlue; }
        double getLineSize(void) const { return _lineSize; }
    };
}
