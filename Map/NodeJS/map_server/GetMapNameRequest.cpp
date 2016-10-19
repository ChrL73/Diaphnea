#include "GetMapNameRequest.h"
#include "MapData.h"
#include "RequestTypeEnum.h"
#include "Map.h"

namespace map_server
{
    void GetMapNameRequest::execute(void)
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        const Map *map = mapData->getMap(_mapId);
        if (map != 0 && _sendResponse)
        {
            std::string name = map->getName(_languageId);
            MapData::unlock();

            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::GET_MAP_NAME << " " << name << std::endl;
            _coutMutexPtr->unlock();
        }
        else
        {
            MapData::unlock();
        }
    }
}
