#pragma once

#include <string>

namespace map_server
{
    class TextInfo
    {
    private:
		const std::string _text;
		double _geoWidth;
		double _geoHeight;
		double _leftInPixels;
		double _bottomInPixels;
		bool _ok;

    public:
        TextInfo(double size, double scale, const std::string& text);
		bool ok(void) const { return _ok; }
    };
}
