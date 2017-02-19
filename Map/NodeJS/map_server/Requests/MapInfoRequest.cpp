#include "MapInfoRequest.h"
#include "MapData.h"
#include "Map.h"
#include "MessageTypeEnum.h"
#include "ErrorEnum.h"

namespace map_server
{
    void MapInfoRequest::execute(void)
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        const Map *map = mapData->getMap(_mapId);
        if (map != 0)
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

			_coutMutexPtr->lock();
			std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
				<< ",\"message\":\"Unknown map Id ('" << _mapId << "') in MAP_INFO request\"}" << std::endl;
			_coutMutexPtr->unlock();
        }
    }
}
