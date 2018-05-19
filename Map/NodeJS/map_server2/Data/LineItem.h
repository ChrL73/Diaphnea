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
        int _lineAlpha;
        int _lineRed;
        int _lineGreen;
        int _lineBlue;
        double _lineSize;
        const MultipointItem * const _multipointItem;

        int getElementLookId(void) const { return _lineLookId; }
        bool hasResolution(void) const { return true; }

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

        void updateLook(const PolygonLook *polygonLook);

        int getLineLookId(void) const { return _lineLookId; }
        int getLineZIndex(void) const { return _lineZIndex; }
        int getLineAlpha(void) const { return _lineAlpha; }
        int getLineRed(void) const { return _lineRed; }
        int getLineGreen(void) const { return _lineGreen; }
        int getLineBlue(void) const { return _lineBlue; }
        double getLineSize(void) const { return _lineSize; }

        const PointList *getPointList(int resolutionIndex) const;

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
