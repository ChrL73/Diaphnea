#pragma once

#include <string>

namespace map_server
{
    class SvgLineInfo
    {
    private:
        const std::string _text;
        const double _x;
        const double _y;

    public:
        SvgLineInfo(const std::string& text, double x, double y) : _text(text), _x(x), _y(y) {}

        const std::string& getText(void) const { return _text; }
        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
    };
}
