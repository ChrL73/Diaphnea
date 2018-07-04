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
    class Potential
    {
    private:
        double _value;
        double _sum;
        double _max;

        static constexpr double _alpha = 0.2;

        // If _alpha = 0.0: Potential(v1) + Potential(v2) + .. + Potential(vn) = Potential(max(v1, v2, ..., vn))
        // If _alpha = 1.0: Potential(v1) + Potential(v2) + .. + Potential(vn) = Potential(v1 + v2 + ... + vn)

    public:
        Potential(void) : _value(0.0), _sum(0.0), _max(0.0) {}
        Potential(double value);

        void add(double value);
        Potential& operator+=(const Potential& p);
        //const Potential operator+(const Potential& p) const;
        double getValue(void) const { return _value; }
    };
}
