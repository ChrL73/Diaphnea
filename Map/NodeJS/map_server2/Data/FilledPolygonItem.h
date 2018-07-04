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

#include "MapItem.h"

#include <set>

namespace map_server
{
    class MultipointItem;
    class PolygonElement;
    class PolygonLook;
    class PointList;
    class Point;

    class FilledPolygonItem : public MapItem
    {
    private:
        const int _fillLookId;
        const int _fillZIndex;
        const char * const _fillColor;
        const double _fillOpacity;

        const MultipointItem * const _multipointItem;
        std::vector<const Point *> _pointVector;
        std::set<double> **_intersections;
        int _height;
        double _yMinP;
        double _yMaxP;

        int getElementLookId(void) const { return _fillLookId; }
        bool hasResolution(void) const { return true; }
        int getZIndex(void) const { return _fillZIndex; }

    public:
        FilledPolygonItem(const PolygonElement *polygonElement, const NameTranslation *name, const MultipointItem *multipointItem, const PolygonLook *polygonLook);
        ~FilledPolygonItem();

        int getFillLookId(void) const { return _fillLookId; }
        int getFillZIndex(void) const { return _fillZIndex; }
        const char *getFillColor(void) const { return _fillColor; }
        double getFillOpacity(void) const { return _fillOpacity; }

        const PointList *getPointList(int resolutionIndex) const;
        void addPoint(double x, double y);
        void setIntersections(double height, double width);

        double getYMinP(void) const { return _yMinP; }
        double getYMaxP(void) const { return _yMaxP; }
        std::set<double> *getIntersections(int y);
    };
}
