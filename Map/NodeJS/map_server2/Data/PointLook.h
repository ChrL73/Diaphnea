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

#include "MapData.h"

namespace map_server
{
    class PointLook
    {
    private:
        int _lookId;

        int _pointZIndex;
        double _pointSize;
        double _pointOpacity;
        int _pointColor; // String

        int _textColor; // String
        double _textSize;
        double _textOpacity;

    public:
        int getLookId(void) const { return _lookId; }

        int getPointZIndex(void) const { return _pointZIndex; }
        double getPointSize(void) const { return _pointSize; }
        double getPointOpacity(void) const { return _pointOpacity; }
        const char *getPointColor(void) const { return reinterpret_cast<const char *>(strings) + _pointColor; }

        const char *getTextColor(void) const { return reinterpret_cast<const char *>(strings) + _textColor; }
        double getTextSize(void) const { return _textSize; }
        double getTextOpacity(void) const { return _textOpacity; }
    };
}
