#pragma once

#include "MapData.h"

namespace map_server
{
    class PointList
    {
    private:
        // Array of 6*n doubles (x1_1, y1_1, x2_1, y2_1, x_1, y_1,  ... , x1_n, y1_n, x2_n, y2_n, x_n, y_n)
        int _pointCount; // n
        int _padding;
        const double *_points;

    public:
        int getPointCount(void) const { return _pointCount; }
        const double *getPoint(int i) const { return _points + 6 * i; }
    };
}
