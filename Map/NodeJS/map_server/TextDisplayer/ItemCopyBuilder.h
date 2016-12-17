#pragma once

namespace map_server
{
    class MapItem;

    class ItemCopyBuilder
    {
    private:
        const MapItem * const _item;
        const double _size;
		const double _textSize;
        const int _resolutionIndex;

    public:
        ItemCopyBuilder(const MapItem *item, double size, double textSize, int resolutionIndex) :
            _item(item), _size(size), _textSize(textSize), _resolutionIndex(resolutionIndex) {}

        const MapItem *getItem(void) const { return _item; }
        double getSize(void) const { return _size; }
		double getTextSize(void) const { return _textSize; }
        int getResolutionIndex(void) const { return _resolutionIndex; }
    };
}
