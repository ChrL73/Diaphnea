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
    class LineLook
    {
    private:
        int _lookId;

        int _lineZIndex;
        double _lineSize;
        double _lineOpacity;
        int _lineColor; // String

        int _textColor; // String
        double _textSize;
        double _textOpacity;

    public:
        int getLookId(void) const { return _lookId; }

        int getLineZIndex(void) const { return _lineZIndex; }
        double getLineSize(void) const { return _lineSize; }
        double getLineOpacity(void) const { return _lineOpacity; }
        const char *getLineColor(void) const { return reinterpret_cast<const char *>(strings) + _lineColor; }

        const char *getTextColor(void) const { return reinterpret_cast<const char *>(strings) + _textColor; }
        double getTextSize(void) const { return _textSize; }
        double getTextOpacity(void) const { return _textOpacity; }
    };
}
