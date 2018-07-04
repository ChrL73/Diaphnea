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
    class SvgCurveInfo
    {
    private:
        const double _x1;
        const double _y1;
        const double _x2;
        const double _y2;
        const double _x;
        const double _y;

    public:
        SvgCurveInfo(double x1, double y1, double x2, double y2, double x, double y) :
            _x1(x1), _y1(y1), _x2(x2), _y2(y2), _x(x), _y(y) {}

        double getX1(void) const { return _x1; }
        double getY1(void) const { return _y1; }
        double getX2(void) const { return _x2; }
        double getY2(void) const { return _y2; }
        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
    };
 }

