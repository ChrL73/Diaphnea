#include "LineItem.h"
#include "MultipointItem.h"
#include "LineLook.h"
#include "PolygonLook.h"
#include "LineElement.h"
#include "Point.h"

#include <cmath>

namespace map_server
{
    LineItem::LineItem(const LineElement *lineElement, const NameTranslation *name, const MultipointItem *multipointItem, const LineLook *lineLook, int resolutionIndex) :
        MapItem(multipointItem->getItemId(), lineElement->getElementId(), lineElement->getImportance(), multipointItem->getXMin(), multipointItem->getXMax(),
        multipointItem->getYMin(), multipointItem->getYMax(), 3 * lineLook->getLookId(), lineLook->getTextAlpha(),
        lineLook->getTextRed(), lineLook->getTextGreen(), lineLook->getTextBlue(), lineLook->getTextSize(), lineElement->getFramingLevel(), name),
        _lineLookId(3 * lineLook->getLookId() + 1), _lineZIndex(lineLook->getLineZIndex()),
        _lineAlpha(lineLook->getLineAlpha()), _lineRed(lineLook->getLineRed()),
        _lineGreen(lineLook->getLineGreen()), _lineBlue(lineLook->getLineBlue()),
        _lineSize(lineLook->getLineSize()), _pointList(multipointItem->getPointList(resolutionIndex))
    {
    }

    LineItem::LineItem(const MultipointItem *multipointItem, const PolygonLook *polygonLook) :
        MapItem(multipointItem->getItemId(), "", -1.0, multipointItem->getXMin(), multipointItem->getXMax(),
        multipointItem->getYMin(), multipointItem->getYMax(),-1, 0, 0, 0, 0, 0.0, -1, 0),
        _lineLookId(3 * polygonLook->getLookId() + 1), _lineZIndex(polygonLook->getContourZIndex()),
        _lineAlpha(polygonLook->getContourAlpha()), _lineRed(polygonLook->getContourRed()),
        _lineGreen(polygonLook->getContourGreen()), _lineBlue(polygonLook->getContourBlue()),
        _lineSize(polygonLook->getContourSize()), _pointList(0)
    {
    }

    void LineItem::updateLook(const PolygonLook *polygonLook)
    {
        if (polygonLook->getContourZIndex() < _lineZIndex)
        {
            _lineZIndex = polygonLook->getContourZIndex();
            _lineAlpha = polygonLook->getContourAlpha();
            _lineRed = polygonLook->getContourRed();
            _lineGreen = polygonLook->getContourGreen();
            _lineBlue = polygonLook->getContourBlue();
            _lineSize = polygonLook->getContourSize();
            _lineLookId = 3 * polygonLook->getLookId() + 1;
        }
    }

    void LineItem::addPoint(double x, double y, bool newLine)
    {
        // Avoid the case where x or y is an integer (adding 0.01 pixel to x or y will not have any visible impact)
        double a;
        if (modf(y, &a) == 0.0) y += 0.01;
        if (modf(x, &a) == 0.0) x += 0.01;

        if (_pointVector.empty() || newLine) _pointVector.push_back(std::vector<const Point *>());
        _pointVector.back().push_back(new Point(x, y));
    }
}
