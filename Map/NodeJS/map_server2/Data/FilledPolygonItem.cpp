#include "FilledPolygonItem.h"
#include "MultipointItem.h"

namespace map_server
{
    FilledPolygonItem::FilledPolygonItem(const MultipointItem *multipointItem) :
        _itemId(multipointItem->getItemId()), _xMin(multipointItem->getXMin()), _xMax(multipointItem->getXMax()),
        _yMin(multipointItem->getYMin()), _yMax(multipointItem->getYMax())
    {
    }
}
