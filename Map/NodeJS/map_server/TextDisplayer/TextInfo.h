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
		double _width;
		double _height;
		double _left;
		double _bottom;
		bool _ok;

    public:
        TextInfo(const std::string& text, double fontSize, FT_Face face);
		bool ok(void) const { return _ok; }
    };
}
