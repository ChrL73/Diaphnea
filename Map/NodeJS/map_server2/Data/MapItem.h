#pragma once

#include <string>

namespace map_server
{
    class MapItem
    {
    private:
        const int _itemId;
        const std::string _elementId;

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

        const int _framingLevel;

    public:
        MapItem(int itemId, const std::string& elementId, double xMin, double xMax, double yMin, double yMax,
                int textLookId, int textAlpha, int textRed, int textGreen, int textBlue, double textSize, int framingLevel) :
            _itemId(itemId), _elementId(elementId), _xMin(xMin), _xMax(xMax), _yMin(yMin), _yMax(yMax),
            _textLookId(textLookId), _textAlpha(textAlpha), _textRed(textRed), _textGreen(textGreen),
            _textBlue(textBlue), _textSize(textSize), _framingLevel(framingLevel) {}

        virtual ~MapItem() {}

        int getItemId(void) const { return _itemId; }
        const std::string& getElementId(void) const { return _elementId; }

        double getXMin(void) const { return _xMin; }
        double getXMax(void) const { return _xMax; }
        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }

        int getTextLookId(void) const { return _textLookId; }
        int getTextAlpha(void) const { return _textAlpha; }
        int getTextRed(void) const { return _textRed; }
        int getTextGreen(void) const { return _textGreen; }
        int getTextBlue(void) const { return _textBlue; }
        double getTextSize(void) const { return _textSize; }

        int getFramingLevel(void) const { return _framingLevel; }

        virtual int getElementLookId(void) const = 0;
        virtual bool hasResolution(void) const = 0;
    };
}
