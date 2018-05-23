#include "PointItem.h"
#include "PointElement.h"
#include "PointLook.h"

namespace map_server
{
    PointItem::PointItem(const PointElement *pointElement, const NameTranslation *name, const PointLook *pointLook) :
        MapItem(pointElement->getItemId(), pointElement->getElementId(), pointElement->getImportance(), pointElement->getX(), pointElement->getX(),
        pointElement->getY(), pointElement->getY(), 3 * pointLook->getLookId(),
        pointLook->getTextColor(), pointLook->getTextOpacity(), pointLook->getTextSize(), pointElement->getFramingLevel(), name),
        _pointLookId(3 * pointLook->getLookId() + 1), _pointZIndex(pointLook->getPointZIndex()),
        _pointColor(pointLook->getPointColor()), _pointOpacity(pointLook->getPointOpacity()),
        _pointSize(pointLook->getPointSize())
    {
    }
}
