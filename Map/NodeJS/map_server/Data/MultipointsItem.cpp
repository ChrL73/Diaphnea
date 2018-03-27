#include "MultipointsItem.h"
#include "PointVector.h"
#include "Point.h"

namespace map_server
{
    MultipointsItem::MultipointsItem(int id, int resolutionCount, IMap *iMap, double xMin, double xMax, double yMin, double yMax) :
            MapItem(id, resolutionCount), _iMap(iMap), _resolutionCount(resolutionCount)
    {
        _xMin = xMin;
        _xMax = xMax;
        _yMin = yMin;
        _yMax = yMax;
    }

    MultipointsItem::~MultipointsItem()
    {
        int i, n = _pointVectorVector.size();
        for (i = 0; i < n; ++i) delete _pointVectorVector[i];
    }

    void MultipointsItem::addPointVector(const mongo::OID& pointListId)
    {
        _pointVectorVector.push_back(new PointVector(_iMap, getId(), pointListId));
    }

    void MultipointsItem::addPointArray(std::stringstream& s, int resolutionIndex) const
    {
        int i, n = _pointVectorVector[resolutionIndex]->getPointCount();
        for (i = 0; i < n; ++i)
        {
            if (i != 0) s << ",";
            const Point *point =  _pointVectorVector[resolutionIndex]->getPoint(i);
            s << "{\"x\":" << point->getX() << ",\"y\":" << point->getY() << "}";
        }
    }
}
