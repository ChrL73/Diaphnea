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

#include "ElementTypeEnum.h"

namespace map_server
{
    class ElementInfo
    {
    private:
        const ElementTypeEnum _type;
        const int _offset;

    public:
        ElementInfo(ElementTypeEnum type, int offset) : _type(type), _offset(offset) {}

        ElementTypeEnum getType(void) const { return _type; }
        int getOffset(void) const { return _offset; }
    };
}
