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

#include "FilledPolygonItem.h"
#include "MultipointItem.h"
#include "PolygonElement.h"
#include "PolygonLook.h"
#include "Point.h"

#include <cmath>

namespace map_server
{
    FilledPolygonItem::FilledPolygonItem(const PolygonElement *polygonElement, const NameTranslation *name, const MultipointItem *multipointItem, const PolygonLook *polygonLook) :
        MapItem(multipointItem->getItemId(), polygonElement->getElementId(), polygonElement->getImportance(), multipointItem->getXMin(), multipointItem->getXMax(),
        multipointItem->getYMin(), multipointItem->getYMax(), 3 * polygonLook->getLookId(),
        polygonLook->getTextColor(), polygonLook->getTextOpacity(), polygonLook->getTextSize(), polygonElement->getFramingLevel(), name),
        _fillLookId(3 * polygonLook->getLookId() + 2), _fillZIndex(polygonLook->getFillZIndex()),
        _fillColor(polygonLook->getFillColor()), _fillOpacity(polygonLook->getFillOpacity()), _multipointItem(multipointItem),
        _intersections(0), _height(0)
    {
    }

    FilledPolygonItem::~FilledPolygonItem()
    {
        int i, n = _pointVector.size();
        for (i = 0; i < n; ++i) delete _pointVector[i];

        for (i = 0; i < _height; ++i) delete _intersections[i];
        delete[] _intersections;
    }

    const PointList *FilledPolygonItem::getPointList(int resolutionIndex) const
    {
        return _multipointItem->getPointList(resolutionIndex);
    }

    void FilledPolygonItem::addPoint(double x, double y)
    {
        // Avoid the case where y is an integer (adding 0.01 pixel to y will not have any visible impact)
        double a;
        if (modf(y, &a) == 0.0) y += 0.01;

        _pointVector.push_back(new Point(x, y));
    }

    std::set<double> *FilledPolygonItem::getIntersections(int y)
    {
        if (y < 0 || y >= _height) return 0;
        return _intersections[y];
    }

    void FilledPolygonItem::setIntersections(double height, double width)
    {
        if (_intersections != 0) return;

        _yMinP = height - 1.0;
        _yMaxP = 0.0;

        _height = static_cast<int>(floor(height));
        _intersections = new std::set<double> *[_height];
        int i;
        for (i = 0; i < _height; ++i) _intersections[i] = 0;

        int n = _pointVector.size();
        for (i = 0; i < n; ++i)
        {
            const Point *point1 = _pointVector[i];
            double x1 = point1->getX();
            double y1 = point1->getY();

            if (x1 > 0.0 && x1 < width)
            {
                if (y1 < _yMinP) _yMinP = y1;
                if (y1 > _yMaxP) _yMaxP = y1;
            }

            const Point *point2;
            if (i < n - 1) point2 = _pointVector[i + 1];
            else point2 = _pointVector[0];
            double x2 = point2->getX();
            double y2 = point2->getY();

            double yMin, yMax;
            if (y1 > y2)
            {
                yMin = ceil(y2);
                yMax = floor(y1);
            }
            else
            {
                yMin = ceil(y1);
                yMax = floor(y2);
            }

            if (yMin <= yMax && yMax >= 0.0 && yMin <= height - 1.0)
            {
                if (yMin < 0.0 ) yMin = 0.0;
                if (yMax > _height - 1.0) yMax = _height - 1.0;

                double y;
                double a = (x2 - x1) / (y2- y1);
                for (y = yMin; y <= yMax; ++y)
                {
                    double x = x1 + a * (y - y1);
                    int yI = static_cast<int>(floor(y));
                    if (_intersections[yI] == 0) _intersections[yI] = new std::set<double>;

                    // If the set already contains 'x': the new 'x' is not inserted into the set and the old 'x' is erased from the set
                    // (because the pairs of identical intersections must not be taken into account:
                    // case of a pair of 2 identical line segments used to link 2 distincts contours of the item)
                    if (!_intersections[yI]->insert(x).second) _intersections[yI]->erase(x);
                }
            }
        }

        // Pairs of identical intersections must not be taken into account
        // Above verification is not sufficient because of rounding imprecision
        // -> Erase 2 intersections if they differ from less than 0.00001 pixel
        for (i = 0; i < _height; ++i)
        {
            if (_intersections[i] != 0)
            {
                std::vector<double> toErase;
                double x0 = 1e100;

                std::set<double>::iterator it = _intersections[i]->begin();
                for (; it != _intersections[i]->end(); ++it)
                {
                    double x = *it;
                    if (x - x0 < 0.00001 && x - x0 > -0.00001)
                    {
                        toErase.push_back(x);
                        toErase.push_back(x0);
                        x0 = 1e100;
                    }
                    else
                    {
                        x0 = x;
                    }
                }

                int j;
                n = toErase.size();
                for (j = 0; j < n; ++j) _intersections[i]->erase(toErase[j]);
            }
        }

        if (_yMinP < 0.0) _yMinP = 0.0;
        if (_yMaxP > height - 1.0) _yMaxP = height - 1.0;
    }
}
