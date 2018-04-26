#include "PointItem.h"
#include "PointElement.h"

namespace map_server
{
    PointItem::PointItem(const PointElement *pointElement) :
        _itemId(pointElement->getItemId()), _elementId(pointElement->getElementId()), _x(pointElement->getX()), _y(pointElement->getY())
    {
    }
}
