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
    class PolygonLook
    {
    private:
        int _lookId;

        int _contourZIndex;
        double _contourSize;
        double _contourOpacity;
        int _contourColor; // String

        int _textColor; // String
        double _textSize;
        double _textOpacity;

        int _fillZIndex;
        int _fillColor; // String
        double _fillOpacity;

    public:
        int getLookId(void) const { return _lookId; }

        int getContourZIndex(void) const { return _contourZIndex; }
        double getContourSize(void) const { return _contourSize; }
        double getContourOpacity(void) const { return _contourOpacity; }
        const char *getContourColor(void) const { return reinterpret_cast<const char *>(strings) + _contourColor; }

        const char *getTextColor(void) const { return reinterpret_cast<const char *>(strings) + _textColor; }
        double getTextSize(void) const { return _textSize; }
        double getTextOpacity(void) const { return _textOpacity; }

        int getFillZIndex(void) const { return _fillZIndex; }
        const char *getFillColor(void) const { return reinterpret_cast<const char *>(strings) + _fillColor; }
        double getFillOpacity(void) const { return _fillOpacity; }
    };
}
