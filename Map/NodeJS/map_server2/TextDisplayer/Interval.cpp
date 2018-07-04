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

#include "Interval.h"

#include <utility>

namespace map_server
{
    const Interval Interval::getIntersection(const Interval& i) const
    {
        if (_emptySet || i._emptySet) return std::move(Interval());

        if (i._a < _a)
        {
            if (i._b < _b) return std::move(Interval(_a, i._b));
            return std::move(Interval(_a, _b));
        }

        if (_b < i._b) return std::move(Interval(i._a, _b));
        return std::move(Interval(i._a, i._b));
    }
}
