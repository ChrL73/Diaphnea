#pragma once

#include <string>

typedef struct FT_FaceRec_ *FT_Face;

namespace map_server
{
    class TextInfo
    {
    private:
		const std::string _text;
		double _fontSize;
		double _geoWidth;
		double _geoHeight;
		double _leftInPixels;
		double _bottomInPixels;
		bool _ok;

    public:
        TextInfo(double size, double scale, const std::string& text, FT_Face face);
		bool ok(void) const { return _ok; }
    };
}
