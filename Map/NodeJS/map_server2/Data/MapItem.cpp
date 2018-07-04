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

#include "MapItem.h"
#include "RepulsiveCenter.h"
#include "TextInfo.h"

namespace map_server
{
    MapItem::~MapItem()
    {
        int i, n = _repulsiveCenterVector.size();
        for (i = 0; i < n; ++i) delete _repulsiveCenterVector[i];

        n = _textInfoVector.size();
        for (i = 0; i < n; ++i) delete _textInfoVector[i];
    }
}
