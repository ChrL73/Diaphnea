#pragma once

#include "Look.h"

namespace map_server
{
    class PointLook : public Look
    {
    private:
        const int _pointZIndex;
        const int _pointAlpha;
        const int _pointRed;
        const int _pointGreen;
        const int _pointBlue;
        const double _pointSize;

    public:
        PointLook(const std::string& id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize,
                  int pointZIndex, int pointAlpha, int pointRed, int pointGreen, int pointBlue, double pointSize) :
            Look(id, textAlpha, textRed, textGreen, textBlue, textSize),
            _pointZIndex(pointZIndex), _pointAlpha(pointAlpha), _pointRed(pointRed), _pointGreen(pointGreen), _pointBlue(pointBlue), _pointSize(pointSize) {}

        int getPointZIndex(void) const { return _pointZIndex; }
        int getPointAlpha(void) const { return _pointAlpha; }
        int getPointRed(void) const { return _pointRed; }
        int getPointGreen(void) const { return _pointGreen; }
        int getPointBlue(void) const { return _pointBlue; }
        double getPointSize(void) const { return _pointSize; }
    };
}

