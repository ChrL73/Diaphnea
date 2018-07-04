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

#include <string>

namespace map_server
{
    class TextInfoLine
    {
    private:
        const std::string _text;
        double _width;
        double _xOffset;
        double _yOffset;

    public:
        TextInfoLine(const std::string& text) : _text(text) {}

        void setWidth(double width) { _width = width; }
        void setXOffset(double xOffset) { _xOffset = xOffset; }
        void setYOffset(double yOffset) { _yOffset = yOffset; }

        const std::string& getText(void) const { return _text; }
        double getWidth(void) const { return _width; }
        double getXOffset(void) const { return _xOffset; }
        double getYOffset(void) const { return _yOffset; }

    };
}
