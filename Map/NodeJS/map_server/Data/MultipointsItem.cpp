#include "MultipointsItem.h"
#include "PointVector.h"
#include "Point.h"

namespace map_server
{
	MultipointsItem::~MultipointsItem()
	{
		int i, n = _pointVectorVector.size();
		for (i = 0; i < n; ++i) delete _pointVectorVector[i];
	}

    void MultipointsItem::addPointVector(const mongo::OID& pointListId)
    {
        _pointVectorVector.push_back(new PointVector(_iMap, getId(), pointListId));
    }

    void MultipointsItem::addPoint(const Point *point)
    {
        if (point->getX() < _xMin) _xMin = point->getX();
        if (point->getX() > _xMax) _xMax = point->getX();
        if (point->getY() < _yMin) _yMin = point->getY();
        if (point->getY() > _yMax) _yMax = point->getY();
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
