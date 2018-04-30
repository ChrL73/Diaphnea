#pragma once

#include <string>

namespace map_server
{
    class PointElement;

    class PointItem
    {
    private:
        const int _itemId;
        const std::string _elementId;
        const double _x;
        const double _y;

        const int _textLookId;
        const int _textAlpha;
        const int _textRed;
        const int _textGreen;
        const int _textBlue;
        const double _textSize;

        const int _pointLookId;
        const int _pointZIndex;
        const int _pointAlpha;
        const int _pointRed;
        const int _pointGreen;
        const int _pointBlue;
        const double _pointSize;

    public:
        PointItem(const PointElement *pointElement, int lookIndex);

        int getItemId(void) const { return _itemId; }
        const std::string& getElementId(void) const { return _elementId; }
        double getX(void) const { return _x; }
        double getY(void) const { return _y; }

        int getTextLookId(void) const { return _textLookId; }
        int getTextAlpha(void) const { return _textAlpha; }
        int getTextRed(void) const { return _textRed; }
        int getTextGreen(void) const { return _textGreen; }
        int getTextBlue(void) const { return _textBlue; }
        double getTextSize(void) const { return _textSize; }

        int getPointLookId(void) const { return _pointLookId; }
        int getPointZIndex(void) const { return _pointZIndex; }
        int getPointAlpha(void) const { return _pointAlpha; }
        int getPointRed(void) const { return _pointRed; }
        int getPointGreen(void) const { return _pointGreen; }
        int getPointBlue(void) const { return _pointBlue; }
        double getPointSize(void) const { return _pointSize; }
    };
}

