#pragma once

namespace map_server
{
    class MapItem;

    class ItemCopyBuilder
    {
    private:
        const MapItem * const _item;
        const double _size;
        const int _resolutionIndex;

    public:
        ItemCopyBuilder(const MapItem *item, double size, int resolutionIndex) :
            _item(item), _size(size), _resolutionIndex(resolutionIndex) {}

        const MapItem *getItem(void) const { return _item; }
        double getSize(void) const { return _size; }
        int getResolutionIndex(void) const { return _resolutionIndex; }
    };
}
