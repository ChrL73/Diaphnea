#include "GetMapIdsRequest.h"
#include "MapData.h"
#include "RequestTypeEnum.h"

namespace map_server
{
    void GetMapIdsRequest::execute(void)
    {
        MapData *mapData = MapData::instance();

        _coutMutexPtr->lock();
        std::cout << _socketId << " " << _requestId << " " << map_server::GET_MAP_IDS << " " << mapData->getMapIds() << std::endl;
        _coutMutexPtr->unlock();
    }
}
