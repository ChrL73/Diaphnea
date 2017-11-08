#pragma once

#include <vector>

typedef struct FT_FaceRec_ *FT_Face;

namespace map_server
{
    class ItemLook;
    class TextInfoLine;

    class TextInfo
    {
    private:
		const std::vector<TextInfoLine *> _lineVector;
		const int _lookId;
		const int _zIndex;
        const int _alpha;
        const int _red;
        const int _green;
        const int _blue;
        const char * const _hexColor;

		const double _fontSize;
		double _width;
		double _height;
		bool _ok;
		double _x;
		double _y;

    public:
        TextInfo(const std::vector<TextInfoLine *>& lineVector, double fontSize, const ItemLook *textLook, FT_Face face);
		bool ok(void) const { return _ok; }
		~TextInfo();

		void setX(double x) { _x = x; }
		void setY(double y) { _y = y; }

        const std::vector<TextInfoLine *>& getLineVector(void) const { return _lineVector; }
        double getFontSize(void) const { return _fontSize; }
        int getLookId(void) const { return _lookId; }
		int getZIndex(void) const { return _zIndex; }
		int getAlpha(void) const { return _alpha; }
		int getRed(void) const { return _red; }
		int getGreen(void) const { return _green; }
		int getBlue(void) const { return _blue; }
		const char *getHexColor(void) const { return _hexColor; }
		double getWidth(void) const { return _width; }
		double getHeight(void) const { return _height; }
		double getX(void) const { return _x; }
		double getY(void) const { return _y; }
    };
}
