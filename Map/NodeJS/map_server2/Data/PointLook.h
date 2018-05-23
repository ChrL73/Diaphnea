#pragma once

#include "MapData.h"

namespace map_server
{
    class PointLook
    {
    private:
        int _lookId;

        int _pointZIndex;
        double _pointSize;
        double _pointOpacity;
        int _pointColor; // String

        int _textColor; // String
        double _textSize;
        double _textOpacity;

    public:
        int getLookId(void) const { return _lookId; }

        int getPointZIndex(void) const { return _pointZIndex; }
        double getPointSize(void) const { return _pointSize; }
        double getPointOpacity(void) const { return _pointOpacity; }
        const char *getPointColor(void) const { return reinterpret_cast<const char *>(strings) + _pointColor; }

        const char *getTextColor(void) const { return reinterpret_cast<const char *>(strings) + _textColor; }
        double getTextSize(void) const { return _textSize; }
        double getTextOpacity(void) const { return _textOpacity; }
    };
}
