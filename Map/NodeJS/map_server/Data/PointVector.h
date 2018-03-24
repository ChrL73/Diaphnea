#pragma once

#ifdef __GNUC__
// Avoid this warning: ‘template<class> class std::auto_ptr’ is deprecated
#pragma GCC diagnostic ignored "-Wdeprecated-declarations"
#endif
#include "mongo/client/dbclient.h"
#ifdef __GNUC__
#pragma GCC diagnostic warning "-Wdeprecated-declarations"
#endif

#include <vector>
#include <string>

namespace map_server
{
    class Point;
    class IMap;

    class PointVector
    {
    private:
        IMap * const _iMap;
        const int _itemId;
        std::vector<const Point *> _points;
        mongo::OID _pointListId;
        bool _loaded;

    public:
        PointVector(IMap *iMap, int itemId, const mongo::OID& pointListId) : _iMap(iMap), _itemId(itemId), _pointListId(pointListId), _loaded(false) {}
        ~PointVector();

        int getPointCount(void) const;
        const Point *getPoint(int i) const;
        void load(void);
    };
}
