#pragma once

#include <string>

namespace map_server
{
    class Look
    {
    private:
        const std::string _id;
        const int _textAlpha;
        const int _textRed;
        const int _textGreen;
        const int _textBlue;
        const double _textSize;

    protected:
        Look(const std::string& id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize) :
            _id(id), _textAlpha(textAlpha), _textRed(textRed), _textGreen(textGreen), _textBlue(textBlue), _textSize(textSize) {}

    public:
        virtual ~Look() {}

        const std::string& getId(void) const { return _id; }
        const int getTextAlpha(void) const { return _textAlpha; }
        const int getTextRed(void) const { return _textRed; }
        const int getTextGreen(void) const { return _textGreen; }
        const int getTextBlue(void) const { return _textSize; }
        const double getTextSize(void) const { return _textSize; }
    };
}
