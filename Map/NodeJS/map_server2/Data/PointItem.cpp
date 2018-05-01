#include "PointItem.h"
#include "PointElement.h"
#include "PointLook.h"

namespace map_server
{
    PointItem::PointItem(const PointElement *pointElement, const PointLook *pointLook) :
        MapItem(pointElement->getItemId(), pointElement->getElementId(), pointElement->getX(), pointElement->getX(),
        pointElement->getY(), pointElement->getY(), 3 * pointLook->getLookId(), pointLook->getTextAlpha(),
        pointLook->getTextRed(), pointLook->getTextGreen(), pointLook->getTextBlue(), pointLook->getTextSize(), pointElement->getFramingLevel()),
        _pointLookId(3 * pointLook->getLookId() + 1), _pointZIndex(pointLook->getPointZIndex()),
        _pointAlpha(pointLook->getPointAlpha()), _pointRed(pointLook->getPointRed()),
        _pointGreen(pointLook->getPointGreen()), _pointBlue(pointLook->getPointBlue()),
        _pointSize(pointLook->getPointSize())
    {
    }
}
