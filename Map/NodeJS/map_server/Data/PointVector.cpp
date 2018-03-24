#include "PointVector.h"
#include "Point.h"
#include "IMap.h"

namespace map_server
{
    PointVector::~PointVector()
    {
        int i, n = _points.size();
        for (i = 0; i < n; ++i) delete _points[i];
    }

    int PointVector::getPointCount(void) const
    {
        if (!_loaded) const_cast<PointVector *>(this)->load();
        return _points.size();
    }

    const Point *PointVector::getPoint(int i) const
    {
        if (!_loaded) const_cast<PointVector *>(this)->load();
        return _points[i];
    }

    void  PointVector::load(void)
    {
        _loaded = true;
        if (!_iMap->loadPointVector(_points, _pointListId, _itemId))
        {
            int i, n = _points.size();
            for (i = 0; i < n; ++i) delete _points[i];
            _points.clear();
        }
    }
}
