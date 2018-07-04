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
    class TextPotential
    {
    private:
        double _max;
        double _sum;
        double _n;
        bool _infinite;

        static constexpr double _alpha = 0.0;

    public:
        TextPotential(void) : _max(0.0), _sum(0.0), _n(0.0), _infinite(false) {}
        TextPotential(bool infinite) : _max(0.0), _sum(0.0), _n(0.0), _infinite(infinite) {}

        void add(double x);
        double getMax(void) const;
        double compareTo(const TextPotential& p) const;
    };
}
