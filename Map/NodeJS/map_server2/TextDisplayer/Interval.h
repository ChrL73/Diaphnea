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

#include <vector>

namespace map_server
{
    class Interval
    {
    private:
        double _a;
        double _b;
        bool _emptySet;

    public:
        Interval(double a, double b) : _a(a), _b(b), _emptySet(a > b) {}
        Interval(void) : _a(1.0), _b(0.0), _emptySet(true) {}

        double getA(void) const { return _a; }
        double getB(void) const { return _b; }
        bool isEmpty(void) const { return _emptySet; }

        const Interval getIntersection(const Interval& i) const;

    };
}
