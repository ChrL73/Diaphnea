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

namespace map_server
{
    enum MessageTypeEnum
    {
        MAP_IDS = 0,
        MAP_INFO = 1,
        ELEMENT_INFO = 2,
        ELEMENTS_INFO = 3,
        ITEM_DATA = 4,
        LOOK = 5,
        RENDER = 6,
        TEXT = 7,
        REMOVE_TEXT = 8,
        SVG = 9,
        ERROR_ = 10
    };
}
