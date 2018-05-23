#pragma once

#include "SvgInfo.h"

namespace map_server
{
    class MapItem;

    class SvgItemInfo : public SvgInfo
    {
    private:
        const MapItem * const _item;
        const int _resolutionIndex;

    public:
        SvgItemInfo(const MapItem *item, int resolutionIndex) : _item(item), _resolutionIndex(resolutionIndex) {}

        const MapItem *getItem(void) const { return _item; }
        int getResolutionIndex(void) const { return _resolutionIndex; }

    };
}
