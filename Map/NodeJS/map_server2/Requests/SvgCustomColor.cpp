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

#include "SvgCustomColor.h"

namespace map_server
{
    SvgCustomColor::SvgCustomColor(const std::string& color, const std::string& opacity) : _color(color), _opacity(1.0), _ok(true)
    {
        if (_color.size() != 7)
        {
            _ok = false;
            return;
        }

        if (_color[0] != '#')
        {
            _ok = false;
            return;
        }

        int i;
        for (i = 1; i < 7; ++i)
        {
            if ((_color[i] < '0' || _color[i] > '9') && (_color[i] < 'a' || _color[i] > 'f') && (_color[i] < 'A' || _color[i] > 'F'))
            {
                _ok = false;
                return;
            }
        }

        try
        {
            _opacity = std::stod(opacity);
        }
        catch (...)
        {
            _opacity = 1.0;
            _ok = false;
            return;
        }

        if (_opacity < 0.0) _opacity = 0.0;
        else if (_opacity > 1.0) _opacity = 1.0;
    }
}
