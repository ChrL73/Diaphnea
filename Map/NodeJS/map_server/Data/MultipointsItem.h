#pragma once

#include "MapItem.h"

#if _WIN32
#include <WinSock2.h>
#endif

#ifdef __GNUC__
// Avoid this warning: ‘template<class> class std::auto_ptr’ is deprecated
#pragma GCC diagnostic ignored "-Wdeprecated-declarations"
#endif
#include "mongo/client/dbclient.h"
#ifdef __GNUC__
#pragma GCC diagnostic warning "-Wdeprecated-declarations"
#endif

#include <vector>
#include <map>
#include <sstream>

namespace map_server
{
    class PointVector;
    class Point;
    class IMap;

    class MultipointsItem : public MapItem
    {
    private:
        IMap * const _iMap;
        std::vector<PointVector *> _pointVectorVector;
        bool hasResolution(void) const { return true; }

    protected:
        MultipointsItem(int id, int resolutionCount, IMap *iMap, double xMin, double xMax, double yMin, double yMax);

        const int _resolutionCount;
        void addPointArray(std::stringstream& s, int resolutionIndex) const;

    public:
        virtual ~MultipointsItem();

        const PointVector *getPointVector(int resolutionIndex) const { return _pointVectorVector[resolutionIndex]; }
        void addPointVector(const mongo::OID& pointListId);
    };
}
