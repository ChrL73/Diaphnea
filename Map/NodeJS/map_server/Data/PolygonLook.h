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
        PolygonLook(int id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize, const std::string& textNameJson,
                    int contourZIndex, int contourAlpha, int contourRed, int contourGreen, int contourBlue, double contourSize, const std::string& contourNameJson,
                    int fillZIndex, int fillAlpha, int fillRed, int fillGreen, int fillBlue, const std::string& fillNameJson, IMap *iMap);
        ~PolygonLook();

        const ItemLook *getContourLook(void) const { return _contourLook; }
        const ItemLook *getFillLook(void) const { return _fillLook; }
    };
}

