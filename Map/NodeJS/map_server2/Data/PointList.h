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
    class PointList
    {
    private:
        // Array of 6*n doubles (x1_1, y1_1, x2_1, y2_1, x_1, y_1,  ... , x1_n, y1_n, x2_n, y2_n, x_n, y_n)
        int _pointCount; // n
        int _padding;
        const double *_points;

    public:
        int getPointCount(void) const { return _pointCount; }
        const double *getPoint(int i) const { return _points + 6 * i; }
    };
}
