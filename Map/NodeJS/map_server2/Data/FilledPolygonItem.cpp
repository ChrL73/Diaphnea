#include "FilledPolygonItem.h"
#include "MultipointItem.h"
#include "PolygonElement.h"
#include "PolygonLook.h"

namespace map_server
{
    FilledPolygonItem::FilledPolygonItem(const PolygonElement *polygonElement, const NameTranslation *name, const MultipointItem *multipointItem, const PolygonLook *polygonLook) :
        MapItem(multipointItem->getItemId(), polygonElement->getElementId(), polygonElement->getImportance(), multipointItem->getXMin(), multipointItem->getXMax(),
        multipointItem->getYMin(), multipointItem->getYMax(), 3 * polygonLook->getLookId(),
        polygonLook->getTextAlpha(), polygonLook->getTextRed(), polygonLook->getTextGreen(),
        polygonLook->getTextBlue(), polygonLook->getTextSize(), polygonElement->getFramingLevel(), name),
        _fillLookId(3 * polygonLook->getLookId() + 2), _fillZIndex(polygonLook->getFillZIndex()),
        _fillAlpha(polygonLook->getFillAlpha()), _fillRed(polygonLook->getFillRed()),
        _fillGreen(polygonLook->getFillGreen()), _fillBlue(polygonLook->getFillBlue())
    {
    }
}
