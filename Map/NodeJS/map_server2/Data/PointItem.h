#pragma once

#include <string>

namespace map_server
{
    class PointItem
    {
    private:
        const int _itemId;
        const std::string _elementId;

    public:
        PointItem(int itemId, const std::string& elementId) :
            _itemId(itemId), _elementId(elementId) {}

        int getItemId(void) const { return _itemId; }
        const std::string& getElementId(void) const { return _elementId; }
    };
}

