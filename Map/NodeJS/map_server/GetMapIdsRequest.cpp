#include "GetMapIdsRequest.h"
#include "MapData.h"
#include "RequestTypeEnum.h"

namespace map_server
{
    void GetMapIdsRequest::execute(void)
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        if (_sendResponse)
        {
            std::string ids = mapData->getMapIdsJson();
            MapData::unlock();

            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::GET_MAP_IDS << " " << ids << std::endl;
            _coutMutexPtr->unlock();
        }
        else
        {
            MapData::unlock();
        }
    }
}