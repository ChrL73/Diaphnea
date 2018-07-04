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

#include "RepulsiveCenter.h"
#include "TextDisplayerParameters.h"

#include <cmath>

namespace map_server
{
    RepulsiveCenter::RepulsiveCenter(const TextDisplayerParameters *parameters, double x, double y, double axisDx, double axisDy, double radius1, double radius2, double u0, bool excluding, bool rectangle) :
        _x(x), _y(y), _u0(u0), _excluding(excluding), _rectangle(rectangle)
    {
        double r = sqrt(parameters->getPotentialTableSizeD() / (axisDx * axisDx + axisDy * axisDy)) / parameters->getMaxRepulsionRatio();
        _v11 = axisDx * r / radius1;
        _v12 = axisDy * r / radius1;
        _v21 = -axisDy * r / radius2;
        _v22 = axisDx * r / radius2;
    }
}
