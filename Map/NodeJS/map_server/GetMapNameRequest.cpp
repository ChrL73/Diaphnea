#include "GetMapNameRequest.h"
#include "MapData.h"
#include "RequestTypeEnum.h"
#include "Map.h"

namespace map_server
{
    void GetMapNameRequest::execute(void)
    {
        MapData *mapData = MapData::instance();

        const Map *map = mapData->getMap(_mapId);
        if (map != 0)
        {
            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::GET_MAP_NAME << " " << map->getName(_languageId) << std::endl;
            _coutMutexPtr->unlock();
        }
    }
}
