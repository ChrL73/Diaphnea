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

#include "SvgInfo.h"

#include <vector>

namespace map_server
{
    class SvgLineInfo;
    class TextInfo;

    class SvgTextInfo : public SvgInfo
    {
    private:
        std::vector<const SvgLineInfo *> _lineVector;
        const TextInfo * const _textInfo;
        const int _lookId;
        const char * const _color;
        const double _opacity;
        const double _fontSize;

    public:
        SvgTextInfo(const TextInfo *textInfo, int lookId, const char *color, double opacity, double fontSize) :
            _textInfo(textInfo), _lookId(lookId), _color(color), _opacity(opacity), _fontSize(fontSize) {}

        void addLine(const SvgLineInfo *line) { _lineVector.push_back(line); }

        const std::vector<const SvgLineInfo *>& getLineVector(void) const { return _lineVector; }
        const TextInfo *getTextInfo(void) const { return _textInfo; }
        int getLookId(void) const { return _lookId; }
        const char *getColor(void) const { return _color; }
        double getOpacity(void) const { return _opacity; }
        double getFontSize(void) const { return _fontSize; }
    };
}
