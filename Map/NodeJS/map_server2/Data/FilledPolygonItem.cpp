#include "FilledPolygonItem.h"
#include "MultipointItem.h"
#include "PolygonElement.h"
#include "PolygonLook.h"
#include "Point.h"

#include <cmath>

namespace map_server
{
    FilledPolygonItem::FilledPolygonItem(const PolygonElement *polygonElement, const NameTranslation *name, const MultipointItem *multipointItem, const PolygonLook *polygonLook, int resolutionIndex) :
        MapItem(multipointItem->getItemId(), polygonElement->getElementId(), polygonElement->getImportance(), multipointItem->getXMin(), multipointItem->getXMax(),
        multipointItem->getYMin(), multipointItem->getYMax(), 3 * polygonLook->getLookId(),
        polygonLook->getTextAlpha(), polygonLook->getTextRed(), polygonLook->getTextGreen(),
        polygonLook->getTextBlue(), polygonLook->getTextSize(), polygonElement->getFramingLevel(), name),
        _fillLookId(3 * polygonLook->getLookId() + 2), _fillZIndex(polygonLook->getFillZIndex()),
        _fillAlpha(polygonLook->getFillAlpha()), _fillRed(polygonLook->getFillRed()),
        _fillGreen(polygonLook->getFillGreen()), _fillBlue(polygonLook->getFillBlue()), _pointList(multipointItem->getPointList(resolutionIndex))
    {
    }

    void FilledPolygonItem::addPoint(double x, double y)
    {
        // Avoid the case where y is an integer (adding 0.01 pixel to y will not have any visible impact)
        double a;
        if (modf(y, &a) == 0.0) y += 0.01;

        _pointVector.push_back(new Point(x, y));
    }
}
