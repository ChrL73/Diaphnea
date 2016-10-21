#include "GetElementInfoRequest.h"
#include "MapData.h"
#include "Map.h"
#include "RequestTypeEnum.h"
#include "MapElement.h"

namespace map_server
{
    void GetElementInfoRequest::execute()
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        Map *map = mapData->getMap(_mapId);
        if (map != 0 && _sendResponse)
        {
            const MapElement *element = map->getElement(_elementId);
            if (element != 0)
            {
                std::string info = element->getInfoJson();
                MapData::unlock();

                _coutMutexPtr->lock();
                std::cout << _socketId << " " << _requestId << " " << map_server::GET_ELEMENT_INFO << " " << info << std::endl;
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
