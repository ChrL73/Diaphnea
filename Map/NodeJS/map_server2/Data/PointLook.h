#pragma once

namespace map_server
{
    class PointLook
    {
    private:
        int _lookId;

        int _pointZIndex;
        int _pointAlpha;
        int _pointRed;
        int _pointGreen;
        int _pointBlue;
        double _pointSize;

        int _textAlpha;
        int _textRed;
        int _textGreen;
        int _textBlue;
        double _textSize;

    public:
        int getLookId(void) const { return _lookId; }

        int getPointZIndex(void) const { return _pointZIndex; }
        int getPointAlpha(void) const { return _pointAlpha; }
        int getPointRed(void) const { return _pointRed; }
        int getPointGreen(void) const { return _pointGreen; }
        int getPointBlue(void) const { return _pointBlue; }
        double getPointSize(void) const { return _pointSize; }

        int getTextAlpha(void) const { return _textAlpha; }
        int getTextRed(void) const { return _textRed; }
        int getTextGreen(void) const { return _textGreen; }
        int getTextBlue(void) const { return _textBlue; }
        double getTextSize(void) const { return _textSize; }
    };
}
