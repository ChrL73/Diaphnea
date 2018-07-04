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

#include "TextDisplayerParameters.h"

#include <cmath>

namespace map_server
{
    bool TextDisplayerParameters::_tablesFilled = false;
    double TextDisplayerParameters::_excludingPotentialTable[_potentialTableSize];
    double TextDisplayerParameters::_notExcludingPotentialTable1[_potentialTableSize];
    double TextDisplayerParameters::_notExcludingPotentialTable2[_potentialTableSize];

    TextDisplayerParameters::TextDisplayerParameters()
    {
        if (!_tablesFilled) fillTables();
    }

    void TextDisplayerParameters::fillTables(void)
    {
        double ratio2 = _maxRepulsionRatio * _maxRepulsionRatio;

        int i;
        for (i = 0; i < _potentialTableSize; ++i)
        {
            double r2 = ratio2 * static_cast<double>(i) / _potentialTableSizeD;
            _excludingPotentialTable[i] = exp(1 - r2);
            _notExcludingPotentialTable2[i] = 2.0 * (ratio2 - r2) / ((ratio2 - 1.0) * (1.0 + r2));

            const double a = 0.4;
            const double b = 0.6;
            if (r2 < a) _notExcludingPotentialTable1[i] = 1.0;
            else if (r2 < b) _notExcludingPotentialTable1[i] = (r2 - b) / (a - b);
            else r2 = 0.0;
        }

        _tablesFilled = true;
    }
}
