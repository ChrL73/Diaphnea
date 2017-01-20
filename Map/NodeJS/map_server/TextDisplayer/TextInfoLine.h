#pragma once

#include <string>

namespace map_server
{
    class TextInfoLine
    {
    private:
        const std::string _text;
        double _width;
		double _xOffset;
		double _yOffset;

    public:
        TextInfoLine(const std::string& text) : _text(text) {}

        void setWidth(double width) { _width = width; }
        void setXOffset(double xOffset) { _xOffset = xOffset; }
        void setYOffset(double yOffset) { _yOffset = yOffset; }

        const std::string& getText(void) const { return _text; }
		double getWidth(void) const { return _width; }
		double getXOffset(void) const { return _xOffset; }
		double getYOffset(void) const { return _yOffset; }

    };
}
