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
    class TextDisplayerParameters;

    class RepulsiveCenter
    {
    private:
        const double _x;
        const double _y;
        const double _u0;
        const bool _excluding;
        const bool _rectangle;
        double _v11;
        double _v12;
        double _v21;
        double _v22;

    public:
        RepulsiveCenter(const TextDisplayerParameters *parameters, double x, double y, double axisDx, double axisDy, double radius1, double radius2, double u0, bool excluding, bool rectangle);

        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
        double getU0(void) const { return _u0; }
        bool getExcluding(void) const { return _excluding; }
        bool getRectangle(void) const { return _rectangle; }
        double getV11(void) const { return _v11; }
        double getV12(void) const { return _v12; }
        double getV21(void) const { return _v21; }
        double getV22(void) const { return _v22; }
    };
}
