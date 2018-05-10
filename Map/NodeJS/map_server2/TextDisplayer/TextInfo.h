#pragma once

#include <vector>

typedef struct FT_FaceRec_ *FT_Face;

namespace map_server
{
    class TextInfoLine;

    class TextInfo
    {
    private:
        const std::vector<TextInfoLine *> _lineVector;

        double _width;
        double _height;
        bool _ok;
        double _x;
        double _y;

    public:
        TextInfo(const std::vector<TextInfoLine *>& lineVector, FT_Face face, double fontSize);
        bool ok(void) const { return _ok; }
        ~TextInfo();

        void setX(double x) { _x = x; }
        void setY(double y) { _y = y; }

        const std::vector<TextInfoLine *>& getLineVector(void) const { return _lineVector; }

        double getWidth(void) const { return _width; }
        double getHeight(void) const { return _height; }
        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
    };
}
