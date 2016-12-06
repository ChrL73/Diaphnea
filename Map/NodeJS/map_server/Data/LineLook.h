#pragma once

#include "Look.h"

namespace map_server
{
    class ItemLook;
    class IMap;

    class LineLook : public Look
    {
    private:
        const ItemLook * const _lineLook;

    public:
        LineLook(int id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize,
                 int lineZIndex, int lineAlpha, int lineRed, int lineGreen, int lineBlue, double lineSize, IMap *iMap);
        ~LineLook();

        const ItemLook *getLineLook(void) const { return _lineLook; }
    };
}
