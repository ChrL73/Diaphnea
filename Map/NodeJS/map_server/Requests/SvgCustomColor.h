#pragma once

#include <string>

namespace map_server
{
    class SvgCustomColor
    {
    private:
        const std::string _color;
        double _opacity;
        bool _ok;

    public:
        SvgCustomColor(const std::string& color, const std::string& opacity);

        const std::string& getColor(void) const { return _color; }
        const double getOpacity(void) const { return _opacity; }
        bool ok(void) const { return _ok; }
    };
}
