#pragma once

#include "MapItem.h"

#include <vector>

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
        const PointList * const _pointList;

        int getElementLookId(void) const { return _lineLookId; }
        bool hasResolution(void) const { return true; }

        std::vector<LineItem *> _associatedLineVector;
        std::vector<std::vector<const Point *> > _pointVector;

    public:
        LineItem(const LineElement *lineElement, const NameTranslation *name, const MultipointItem *multipointItem, const LineLook *lineLook, int resolutionIndex);
        LineItem(const MultipointItem *multipointItem, const PolygonLook *polygonLook);

        void updateLook(const PolygonLook *polygonLook);

        int getLineLookId(void) const { return _lineLookId; }
        int getLineZIndex(void) const { return _lineZIndex; }
        int getLineAlpha(void) const { return _lineAlpha; }
        int getLineRed(void) const { return _lineRed; }
        int getLineGreen(void) const { return _lineGreen; }
        int getLineBlue(void) const { return _lineBlue; }
        double getLineSize(void) const { return _lineSize; }

        const PointList *getPointList(void) const { return _pointList; }

        void setAssociatedLineVector(const std::vector<LineItem *>& associatedLineVector) { _associatedLineVector = associatedLineVector; }
        int getAssociatedLineCount(void) const { return _associatedLineVector.size(); }
        LineItem *getAssociatedLine(int i) const { return _associatedLineVector[i];}

        void addPoint(double x, double y, bool newLine);
    };
}
