#pragma once

#include "MapData.h"

namespace map_server
{
    class LineLook
    {
    private:
        int _lookId;

        int _lineZIndex;
        double _lineSize;
        double _lineOpacity;
        int _lineColor; // String

        int _textColor; // String
        double _textSize;
        double _textOpacity;

    public:
        int getLookId(void) const { return _lookId; }

        int getLineZIndex(void) const { return _lineZIndex; }
        double getLineSize(void) const { return _lineSize; }
        double getLineOpacity(void) const { return _lineOpacity; }
        const char *getLineColor(void) const { return reinterpret_cast<const char *>(strings) + _lineColor; }

        const char *getTextColor(void) const { return reinterpret_cast<const char *>(strings) + _textColor; }
        double getTextSize(void) const { return _textSize; }
        double getTextOpacity(void) const { return _textOpacity; }
    };
}
