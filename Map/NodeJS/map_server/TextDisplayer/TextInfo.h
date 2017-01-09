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
		double _xOffset;
		double _yOffset;
		bool _ok;
		double _x;
		double _y;

    public:
        TextInfo(const std::string& text, double fontSize, FT_Face face);
		bool ok(void) const { return _ok; }

		void setX(double x) { _x = x; }
		void setY(double y) { _y = y; }

        const std::string& getText(void) const { return _text; }
		double getFontSize(void) const { return _fontSize; }
		double getWidth(void) const { return _width; }
		double getHeight(void) const { return _height; }
		double getXOffset(void) const { return _xOffset; }
		double getYOffset(void) const { return _yOffset; }
		double getX(void) const { return _x; }
		double getY(void) const { return _y; }
    };
}
