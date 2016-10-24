#pragma once

#include "Look.h"

namespace map_server
{
    class PolygonLook : public Look
    {
    private:
        const int _contourZIndex;
        const int _contourAlpha;
        const int _contourRed;
        const int _contourGreen;
        const int _contourBlue;
        const double _contourSize;
        const double _fillLightRatio;
        const int _fillZIndex;

    public:
        PolygonLook(const std::string& id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize,
                    int contourZIndex, int contourAlpha, int contourRed, int contourGreen, int contourBlue, int contourSize, double fillLightRatio, int fillZIndex) :
            Look(id, textAlpha, textRed, textGreen, textBlue, textSize),
            _contourZIndex(contourZIndex), _contourAlpha(contourAlpha), _contourRed(contourRed), _contourGreen(contourGreen),
            _contourBlue(contourBlue), _contourSize(contourSize), _fillLightRatio(fillLightRatio), _fillZIndex(fillZIndex) {}

        int getPolygonZIndex(void) const { return _contourZIndex; }
        int getPolygonAlpha(void) const { return _contourAlpha; }
        int getPolygonRed(void) const { return _contourRed; }
        int getPolygonGreen(void) const { return _contourGreen; }
        int getPolygonBlue(void) const { return _contourBlue; }
        double getPolygonSize(void) const { return _contourSize; }
        double getFillLightRatio(void) const { return _fillLightRatio; }
        int getFillZIndex(void) const { return _fillZIndex; }
    };
}

