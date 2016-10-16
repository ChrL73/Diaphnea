#include "GetMapIdsRequest.h"
#include "MapData.h"
#include "RequestTypeEnum.h"

namespace map_server
{
    void GetMapIdsRequest::execute(void)
    {
        MapData *mapData = MapData::instance();

        const std::vector<std::string>& mapIdVector = mapData->getMapIdVector();

        int i, n = mapIdVector.size();

        _coutMutexPtr->lock();
        std::cout << _socketId << " " << _requestId << " " << map_server::GET_MAP_IDS << " ";
        for (i = 0; i < n; ++i)
        {
            std::cout << mapIdVector[i];
            if (i != n - 1) std::cout << " ";
        }
        std::cout << std::endl;
        _coutMutexPtr->unlock();
    }
}
