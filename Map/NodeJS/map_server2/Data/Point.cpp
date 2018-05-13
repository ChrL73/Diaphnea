#include "Point.h"
#include "BezierInfo.h"

namespace map_server
{
    Point::Point(double x1, double y1, double x2, double y2, double x, double y) :
        _bezierInfo(new BezierInfo(x1, y1, x2, y2)), _x(x), _y(y)
    {
    }

    Point::~Point()
    {
        delete _bezierInfo;
    }
}
