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

#include <vector>
#include <map>

namespace map_server
{
    class MultipointItem;
    class LineLook;
    class PolygonLook;
    class LineElement;
    class PointList;
    class Point;

    class LineItem : public MapItem
    {
    private:
        int _lineLookId;
        int _lineZIndex;
        const char *_lineColor;
        double _lineOpacity;
        double _lineSize;
        const MultipointItem * const _multipointItem;

        int getElementLookId(void) const { return _lineLookId; }
        bool hasResolution(void) const { return true; }
        int getZIndex(void) const { return _lineZIndex; }

        std::vector<LineItem *> _associatedLineVector;
        std::vector<std::vector<const Point *> > _pointVector;

        std::map<double, double> **_hIntersections;
        int _height;
        double _yMinP;
        double _yMaxP;

        std::map<double, double> **_vIntersections;
        int _width;
        double _xMinP;
        double _xMaxP;

    public:
        LineItem(const LineElement *lineElement, const NameTranslation *name, const MultipointItem *multipointItem, const LineLook *lineLook);
        LineItem(const MultipointItem *multipointItem, const PolygonLook *polygonLook);
        ~LineItem();

        void updateLook(const PolygonLook *polygonLook);

        int getLineLookId(void) const { return _lineLookId; }
        int getLineZIndex(void) const { return _lineZIndex; }
        const char *getLineColor(void) const { return _lineColor; }
        double getLineOpacity(void) const { return _lineOpacity; }
        double getLineSize(void) const { return _lineSize; }

        const PointList *getPointList(int resolutionIndex) const;
        bool cap1Round(void) const;
        bool cap2Round(void) const;

        void setAssociatedLineVector(const std::vector<LineItem *>& associatedLineVector) { _associatedLineVector = associatedLineVector; }
        int getAssociatedLineCount(void) const { return _associatedLineVector.size(); }
        LineItem *getAssociatedLine(int i) const { return _associatedLineVector[i];}

        void addPoint(double x, double y, bool newLine);
        void setIntersections(double height, double width);

        double getYMinP(void) const { return _yMinP; }
        double getYMaxP(void) const { return _yMaxP; }
        std::map<double, double> *getHIntersections(int y);

        double getXMinP(void) const { return _xMinP; }
        double getXMaxP(void) const { return _xMaxP; }
        std::map<double, double> *getVIntersections(int x);
    };
}
