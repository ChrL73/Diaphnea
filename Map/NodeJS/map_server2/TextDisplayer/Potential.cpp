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

#include "Potential.h"

namespace map_server
{
    Potential::Potential(double value)
    {
        if (value > 0.0)
        {
            _value = value;
            _sum = value;
            _max = value;
        }
        else
        {
            _value = 0.0;
            _sum = 0.0;
            _max = 0.0;
        }
    }

    void Potential::add(double value)
    {
        if (value > 0.0)
        {
            _sum += value;
            if (value > _max) _max = value;
            _value = (1.0 - _alpha) * _max + _alpha * _sum;
        }
    }

    Potential& Potential::operator+=(const Potential& p)
    {
        _sum += p._sum;
        if (p._max > _max) _max = p._max;
        _value = (1.0 - _alpha) * _max + _alpha * _sum;

        return *this;
    }

    /*const Potential Potential::operator+(const Potential& p) const
    {
        Potential result = *this;

        result._sum += p._sum;
        if (p._max > result._max) result._max = p._max;
        result._value = (1.0 - _alpha) * result._max + _alpha * result._sum;

        return result;
    }*/
}
