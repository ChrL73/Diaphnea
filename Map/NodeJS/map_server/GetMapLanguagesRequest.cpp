#include "GetMapLanguagesRequest.h"
#include "MapData.h"
#include "Map.h"
#include "RequestTypeEnum.h"

namespace map_server
{
    void GetMapLanguagesRequest::execute(void)
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        const Map *map = mapData->getMap(_mapId);
        if (map != 0 && _sendResponse)
        {
            std::string languages = map->getLanguagesJson();
            MapData::unlock();

            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::GET_MAP_LANGUAGES << " " << languages << std::endl;
            _coutMutexPtr->unlock();
        }
        else
        {
            MapData::unlock();
        }
    }
}
