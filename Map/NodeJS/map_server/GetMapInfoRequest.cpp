#include "GetMapInfoRequest.h"
#include "MapData.h"
#include "Map.h"
#include "MessageTypeEnum.h"

namespace map_server
{
    void GetMapInfoRequest::execute(void)
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        const Map *map = mapData->getMap(_mapId);
        if (map != 0 && _sendResponse)
        {
            std::string info = map->getInfoJson();
            MapData::unlock();

            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::MAP_INFO << " " << info << std::endl;
            _coutMutexPtr->unlock();
        }
        else
        {
            MapData::unlock();
        }
    }
}
