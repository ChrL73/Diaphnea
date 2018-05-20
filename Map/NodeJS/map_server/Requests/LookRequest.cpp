#include "LookRequest.h"
#include "MapData.h"
#include "ItemLook.h"
#include "Map.h"
#include "MessageTypeEnum.h"
#include "ErrorEnum.h"

#include <iostream>

namespace map_server
{
    void LookRequest::execute()
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        Map *map = mapData->getMap(_mapId);
        if (map != 0)
        {
            const ItemLook *look = map->getItemLook(_lookId);
            if (look != 0)
            {
                std::string info = look->getJson();
                MapData::unlock();

                _coutMutexPtr->lock();
                std::cout << _socketId << " " << _requestId << " " << map_server::LOOK << " " << info << std::endl;
                _coutMutexPtr->unlock();
            }
            else
            {
                MapData::unlock();

                _coutMutexPtr->lock();
                std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
                    << ",\"message\":\"Unknown look id ('" << _lookId << "') in LOOK request\"}" << std::endl;
                _coutMutexPtr->unlock();
            }

            flushErrors(map);
        }
        else
        {
            MapData::unlock();

            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
                << ",\"message\":\"Unknown map id ('" << _mapId << "') in LOOK request\"}" << std::endl;
            _coutMutexPtr->unlock();
        }
    }
}
