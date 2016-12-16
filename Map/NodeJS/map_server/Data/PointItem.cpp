#include "PointItem.h"
#include "Point.h"

#include <sstream>

namespace map_server
{
    PointItem::PointItem(int id, const Point *point) : MapItem(id, 1), _point(point)
    {
        _xMin = point->getX();
        _xMax = point->getX();
        _yMin = point->getY();
        _yMax = point->getY();

        std::stringstream s;
        s << "{\"type\":\"point\",\"x\":" << point->getX() << ",\"y\":" << point->getY() << "}";
        _infoJsonVector[0] = s.str();
    }

	PointItem::~PointItem()
	{
		delete _point;
	}
}
