#include "PointItem.h"
#include "PointElement.h"
#include "PointLook.h"

namespace map_server
{
    PointItem::PointItem(const PointElement *pointElement, int lookIndex) :
        _itemId(pointElement->getItemId()), _elementId(pointElement->getElementId()), _x(pointElement->getX()), _y(pointElement->getY()),
        _textLookId(3 * pointElement->getLook(lookIndex)->getLookId()), _textAlpha(pointElement->getLook(lookIndex)->getTextAlpha()),
        _textRed(pointElement->getLook(lookIndex)->getTextRed()), _textGreen(pointElement->getLook(lookIndex)->getTextGreen()),
        _textBlue(pointElement->getLook(lookIndex)->getTextBlue()), _textSize(pointElement->getLook(lookIndex)->getTextSize()),
        _pointLookId(3 * pointElement->getLook(lookIndex)->getLookId() + 1), _pointZIndex(pointElement->getLook(lookIndex)->getPointZIndex()),
        _pointAlpha(pointElement->getLook(lookIndex)->getPointAlpha()), _pointRed(pointElement->getLook(lookIndex)->getPointRed()),
        _pointGreen(pointElement->getLook(lookIndex)->getPointGreen()), _pointBlue(pointElement->getLook(lookIndex)->getPointBlue()),
        _pointSize(pointElement->getLook(lookIndex)->getPointSize())
    {
    }
}
