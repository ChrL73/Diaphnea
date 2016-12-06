#pragma once

#include "Look.h"

namespace map_server
{
    class IMap;

    class PointLook : public Look
    {
    private:
        const ItemLook * const _pointLook;

    public:
        PointLook(int id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize,
                  int pointZIndex, int pointAlpha, int pointRed, int pointGreen, int pointBlue, double pointSize, IMap *iMap);
        ~PointLook();

        const ItemLook *getPointLook(void) const { return _pointLook; }
    };
}

