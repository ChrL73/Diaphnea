#include "LineItem.h"
#include "MultipointItem.h"
#include "LineLook.h"
#include "PolygonLook.h"

namespace map_server
{
    LineItem::LineItem(const MultipointItem *multipointItem, const LineLook *lineLook) :
        _itemId(multipointItem->getItemId()), _xMin(multipointItem->getXMin()), _xMax(multipointItem->getXMax()),
        _yMin(multipointItem->getYMin()), _yMax(multipointItem->getYMax()),
        _textLookId(3 * lineLook->getLookId()), _textAlpha(lineLook->getTextAlpha()),
        _textRed(lineLook->getTextRed()), _textGreen(lineLook->getTextGreen()),
        _textBlue(lineLook->getTextBlue()), _textSize(lineLook->getTextSize()),
        _lineLookId(3 * lineLook->getLookId() + 1), _lineZIndex(lineLook->getLineZIndex()),
        _lineAlpha(lineLook->getLineAlpha()), _lineRed(lineLook->getLineRed()),
        _lineGreen(lineLook->getLineGreen()), _lineBlue(lineLook->getLineBlue()),
        _lineSize(lineLook->getLineSize())
    {
    }

    LineItem::LineItem(const MultipointItem *multipointItem, const PolygonLook *polygonLook) :
        _itemId(multipointItem->getItemId()), _xMin(multipointItem->getXMin()), _xMax(multipointItem->getXMax()),
        _yMin(multipointItem->getYMin()), _yMax(multipointItem->getYMax()),
        _textLookId(-1), _textAlpha(0), _textRed(0), _textGreen(0), _textBlue(0), _textSize(0),
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
        }
    }
}
