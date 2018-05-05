#include "LineItem.h"
#include "MultipointItem.h"
#include "LineLook.h"
#include "PolygonLook.h"
#include "LineElement.h"

namespace map_server
{
    LineItem::LineItem(const LineElement *lineElement, const MultipointItem *multipointItem, const LineLook *lineLook) :
        MapItem(multipointItem->getItemId(), lineElement->getElementId(), lineElement->getImportance(), multipointItem->getXMin(), multipointItem->getXMax(),
        multipointItem->getYMin(), multipointItem->getYMax(), 3 * lineLook->getLookId(), lineLook->getTextAlpha(),
        lineLook->getTextRed(), lineLook->getTextGreen(), lineLook->getTextBlue(), lineLook->getTextSize(), lineElement->getFramingLevel()),
        _lineLookId(3 * lineLook->getLookId() + 1), _lineZIndex(lineLook->getLineZIndex()),
        _lineAlpha(lineLook->getLineAlpha()), _lineRed(lineLook->getLineRed()),
        _lineGreen(lineLook->getLineGreen()), _lineBlue(lineLook->getLineBlue()),
        _lineSize(lineLook->getLineSize())
    {
    }

    LineItem::LineItem(const MultipointItem *multipointItem, const PolygonLook *polygonLook) :
        MapItem(multipointItem->getItemId(), "", -1.0, multipointItem->getXMin(), multipointItem->getXMax(),
        multipointItem->getYMin(), multipointItem->getYMax(),-1, 0, 0, 0, 0, 0.0, -1),
        _lineLookId(3 * polygonLook->getLookId() + 1), _lineZIndex(polygonLook->getContourZIndex()),
        _lineAlpha(polygonLook->getContourAlpha()), _lineRed(polygonLook->getContourRed()),
        _lineGreen(polygonLook->getContourGreen()), _lineBlue(polygonLook->getContourBlue()),
        _lineSize(polygonLook->getContourSize())
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
}
