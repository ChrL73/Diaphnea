#pragma once

#include "Look.h"

namespace map_server
{
    class LineLook : public Look
    {
    private:
        const int _lineZIndex;
        const int _lineAlpha;
        const int _lineRed;
        const int _lineGreen;
        const int _lineBlue;
        const double _lineSize;

    public:
        LineLook(const std::string& id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize,
                 int lineZIndex, int lineAlpha, int lineRed, int lineGreen, int lineBlue, double lineSize) :
            Look(id, textAlpha, textRed, textGreen, textBlue, textSize),
            _lineZIndex(lineZIndex), _lineAlpha(lineAlpha), _lineRed(lineRed), _lineGreen(lineGreen), _lineBlue(lineBlue), _lineSize(lineSize) {}

        int getLineZIndex(void) const { return _lineZIndex; }
        int getLineAlpha(void) const { return _lineAlpha; }
        int getLineRed(void) const { return _lineRed; }
        int getLineGreen(void) const { return _lineGreen; }
        int getLineBlue(void) const { return _lineBlue; }
        double getLineSize(void) const { return _lineSize; }
    };
}
