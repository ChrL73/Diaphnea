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
    enum ErrorEnum
    {
        UNHANDLED_EXCEPTION = 0,
        EMPTY_REQUEST = 6,
        NO_REQUEST_ID = 7,
        NO_REQUEST_TYPE = 8,
        BAD_REQUEST_TYPE = 9,
        NOT_ENOUGH_TOKENS = 10,
        BAD_PARAMETER = 11,
        UNKNOWN_ID = 12,
        FREE_TYPE_INIT_FAILED = 13,
        FONT_NOT_FOUND = 14
    };
}
