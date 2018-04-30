#pragma once

namespace map_server
{
    class PolygonLook
    {
    private:
        int _lookId;

        int _contourZIndex;
        int _contourAlpha;
        int _contourRed;
        int _contourGreen;
        int _contourBlue;
        double _contourSize;

        int _textAlpha;
        int _textRed;
        int _textGreen;
        int _textBlue;
        double _textSize;

        int _fillZIndex;
        int _fillAlpha;
        int _fillRed;
        int _fillGreen;
        int _fillBlue;

    public:
        int getLookId(void) const { return _lookId; }

        int getContourZIndex(void) const { return _contourZIndex; }
        int getContourAlpha(void) const { return _contourAlpha; }
        int getContourRed(void) const { return _contourRed; }
        int getContourGreen(void) const { return _contourGreen; }
        int getContourBlue(void) const { return _contourBlue; }
        double getContourSize(void) const { return _contourSize; }

        int getTextAlpha(void) const { return _textAlpha; }
        int getTextRed(void) const { return _textRed; }
        int getTextGreen(void) const { return _textGreen; }
        int getTextBlue(void) const { return _textBlue; }
        double getTextSize(void) const { return _textSize; }

        int getFillZIndex(void) const { return _fillZIndex; }
        int getFillAlpha(void) const { return _fillAlpha; }
        int getFillRed(void) const { return _fillRed; }
        int getFillGreen(void) const { return _fillGreen; }
        int getFillBlue(void) const { return _fillBlue; }
    };
}