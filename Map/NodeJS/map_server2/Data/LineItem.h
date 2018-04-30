#pragma once

namespace map_server
{
    class MultipointItem;
    class LineLook;
    class PolygonLook;

    class LineItem
    {
    private:
        const int _itemId;

        const double _xMin;
        const double _xMax;
        const double _yMin;
        const double _yMax;

        const int _textLookId;
        const int _textAlpha;
        const int _textRed;
        const int _textGreen;
        const int _textBlue;
        const double _textSize;

        int _lineLookId;
        int _lineZIndex;
        int _lineAlpha;
        int _lineRed;
        int _lineGreen;
        int _lineBlue;
        double _lineSize;

    public:
        LineItem(const MultipointItem *multipointItem, const LineLook *lineLook);
        LineItem(const MultipointItem *multipointItem, const PolygonLook *polygonLook);

        int getItemId(void) const { return _itemId; }

        double getXMin(void) const { return _xMin; }
        double getXMax(void) const { return _xMax; }
        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }

        void updateLook(const PolygonLook *polygonLook);
    };
}
