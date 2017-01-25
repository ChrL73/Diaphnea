#pragma once

#include <vector>

namespace map_server
{
    class MapItem;
    class ItemLook;

    class ItemCopyBuilder
    {
    private:
        const MapItem * const _item;
        const double _size;
		const ItemLook * const _textLook;
        const int _resolutionIndex;
        std::vector<ItemCopyBuilder *> _lineBuilderVector;

    public:
        ItemCopyBuilder(const MapItem *item, double size, const ItemLook *textLook, int resolutionIndex) :
            _item(item), _size(size), _textLook(textLook), _resolutionIndex(resolutionIndex) {}

        const MapItem *getItem(void) const { return _item; }
        double getSize(void) const { return _size; }
		const ItemLook *getTextLook(void) const { return _textLook; }
        int getResolutionIndex(void) const { return _resolutionIndex; }

        void setLineBuilderVector(const std::vector<ItemCopyBuilder *> lineBuilderVector) { _lineBuilderVector = lineBuilderVector; }
        int getLineBuilderCount(void) const { return _lineBuilderVector.size(); }
        ItemCopyBuilder *getLineBuilder(int i) { return _lineBuilderVector[i]; }
    };
}
