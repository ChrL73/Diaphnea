#include "GetElementIdsRequest.h"
#include "MapData.h"
#include "Map.h"
#include "RequestTypeEnum.h"

namespace map_server
{
    void GetElementIdsRequest::execute(void)
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        const Map *map = mapData->getMap(_mapId);
        if (map != 0 && _sendResponse)
        {
            std::string ids = map->getElementIdsJson();
            MapData::unlock();

            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::GET_ELEMENT_IDS << " " << ids << std::endl;
            _coutMutexPtr->unlock();
        }
        else
        {
            MapData::unlock();
        }
    }
}
