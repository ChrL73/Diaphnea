/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

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
