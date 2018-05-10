#pragma once

#include <string>
#include <vector>

namespace map_server
{
    class RepulsiveCenter;
    class NameTranslation;
    class TextInfo;

    class MapItem
    {
    private:
        const int _itemId;
        const std::string _elementId;
        const double _importance;

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

        const NameTranslation * const _name;
        double _fontSize;

        std::vector<const RepulsiveCenter *> _repulsiveCenterVector;
        std::vector<TextInfo *> _textInfoVector;

    public:
        MapItem(int itemId, const std::string& elementId, double importance, double xMin, double xMax, double yMin, double yMax,
                int textLookId, int textAlpha, int textRed, int textGreen, int textBlue, double textSize, int framingLevel, const NameTranslation *name) :
            _itemId(itemId), _elementId(elementId), _importance(importance), _xMin(xMin), _xMax(xMax), _yMin(yMin), _yMax(yMax),
            _textLookId(textLookId), _textAlpha(textAlpha), _textRed(textRed), _textGreen(textGreen),
            _textBlue(textBlue), _textSize(textSize), _framingLevel(framingLevel), _name(name) {}

        virtual ~MapItem() {}

        int getItemId(void) const { return _itemId; }
        const std::string& getElementId(void) const { return _elementId; }
        double getImportance(void) const { return _importance; }

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

        const NameTranslation *getName(void) const { return _name; }
        void setFontSize(double fontSize) { _fontSize = fontSize; }
        double getFontSize(void) const { return _fontSize; }

        virtual int getElementLookId(void) const = 0;
        virtual bool hasResolution(void) const = 0;

        void addRepulsiveCenter(const RepulsiveCenter *repulsiveCenter) { _repulsiveCenterVector.push_back(repulsiveCenter); }
        void addTextInfo(TextInfo *textInfo) { _textInfoVector.push_back(textInfo); }
    };
}
