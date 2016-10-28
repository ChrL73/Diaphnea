#pragma once

#include "Look.h"

namespace map_server
{
    class IMap;

    class PolygonLook : public Look
    {
    private:
        const ItemLook * const _contourLook;
        const ItemLook * const _fillLook;

    public:
        PolygonLook(int id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize,
                    int contourZIndex, int contourAlpha, int contourRed, int contourGreen, int contourBlue, double contourSize, double fillLightRatio, int fillZIndex, IMap *iMap);
        ~PolygonLook();

        const ItemLook *getContourLook(void) const { return _contourLook; }
        const ItemLook *getFillLook(void) const { return _fillLook; }
    };
}

