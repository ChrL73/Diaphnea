#pragma once

#include "MapData.h"

namespace map_server
{
    class PolygonLook
    {
    private:
        int _lookId;

        int _contourZIndex;
        double _contourSize;
        double _contourOpacity;
        int _contourColor; // String

        int _textColor; // String
        double _textSize;
        double _textOpacity;

        int _fillZIndex;
        int _fillColor; // String
        double _fillOpacity;

    public:
        int getLookId(void) const { return _lookId; }

        int getContourZIndex(void) const { return _contourZIndex; }
        double getContourSize(void) const { return _contourSize; }
        double getContourOpacity(void) const { return _contourOpacity; }
        const char *getContourColor(void) const { return reinterpret_cast<const char *>(strings) + _contourColor; }

        const char *getTextColor(void) const { return reinterpret_cast<const char *>(strings) + _textColor; }
        double getTextSize(void) const { return _textSize; }
        double getTextOpacity(void) const { return _textOpacity; }

        int getFillZIndex(void) const { return _fillZIndex; }
        const char *getFillColor(void) const { return reinterpret_cast<const char *>(strings) + _fillColor; }
        double getFillOpacity(void) const { return _fillOpacity; }
    };
}
