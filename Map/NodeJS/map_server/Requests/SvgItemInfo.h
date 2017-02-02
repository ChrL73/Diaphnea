#pragma once

#include "SvgInfo.h"

namespace map_server
{
    class MapItem;
    class ItemLook;

    class SvgItemInfo : public SvgInfo
    {
    private:
        const MapItem * const _item;
        const ItemLook * const _look;
        const int _resolutionIndex;

    public:
        SvgItemInfo(const MapItem *item, const ItemLook *look, int resolutionIndex) : _item(item), _look(look), _resolutionIndex(resolutionIndex) {}

        const MapItem *getItem(void) const { return _item; }
        const ItemLook *getLook(void) const { return _look; }
        int getResolutionIndex(void) const { return _resolutionIndex; }

    };
}
