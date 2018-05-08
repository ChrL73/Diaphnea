#pragma once

#include "MapData.h"

namespace map_server
{
    class ElementName
    {
    private:
        // Array of strings
        int _lineCount;
        int _lines;

    public:
        int getLineCount(void) const { return _lineCount; }
        const char *getLine(int lineIndex) const { return strings + *(intArrays + _lines + lineIndex); }
    };
}
