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
        LineLook(int id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize, const std::string& textNameJson,
                 int lineZIndex, int lineAlpha, int lineRed, int lineGreen, int lineBlue, double lineSize, const std::string& lineNameJson, IMap *iMap);
        ~LineLook();

        const ItemLook *getLineLook(void) const { return _lineLook; }
    };
}
