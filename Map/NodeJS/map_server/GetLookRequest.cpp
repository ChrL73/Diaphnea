#include "GetLookRequest.h"
#include "MapData.h"
#include "ItemLook.h"
#include "Map.h"
#include "MessageTypeEnum.h"

namespace map_server
{
    void GetLookRequest::execute()
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        Map *map = mapData->getMap(_mapId);
        if (map != 0 && _sendResponse)
        {
            const ItemLook *look = map->getItemLook(_lookId);
            if (look != 0)
            {
                std::string info = look->getJson();
                MapData::unlock();

                _coutMutexPtr->lock();
                std::cout << _socketId << " " << _requestId << " " << map_server::GET_LOOK << " " << info << std::endl;
                _coutMutexPtr->unlock();
            }
            else
            {
                MapData::unlock();
            }
        }
        else
        {
            MapData::unlock();
        }
    }
}
