#include "SvgCustomColor.h"

namespace map_server
{
    SvgCustomColor::SvgCustomColor(const std::string& color, const std::string& opacity) : _color(color), _opacity(1.0), _ok(true)
    {
        if (_color.size() != 7)
        {
            _ok = false;
            return;
        }

        if (_color[0] != '#')
        {
            _ok = false;
            return;
        }

        int i;
        for (i = 1; i < 7; ++i)
        {
            if ((_color[i] < '0' || _color[i] > '9') && (_color[i] < 'a' || _color[i] > 'f') && (_color[i] < 'A' || _color[i] > 'F'))
            {
                _ok = false;
                return;
            }
        }

        try
        {
            _opacity = std::stod(opacity);
        }
        catch (...)
        {
            _opacity = 1.0;
            _ok = false;
            return;
        }

        if (_opacity < 0.0) _opacity = 0.0;
        else if (_opacity > 1.0) _opacity = 1.0;
    }
}
