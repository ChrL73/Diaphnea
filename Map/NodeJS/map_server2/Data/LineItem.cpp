#include "LineItem.h"
#include "MultipointItem.h"

namespace map_server
{
    LineItem::LineItem(const MultipointItem *multipointItem) :
        _itemId(multipointItem->getItemId()), _xMin(multipointItem->getXMin()), _xMax(multipointItem->getXMax()),
        _yMin(multipointItem->getYMin()), _yMax(multipointItem->getYMax())
    {
    }
}
