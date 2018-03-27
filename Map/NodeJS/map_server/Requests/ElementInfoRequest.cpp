#include "ElementInfoRequest.h"
#include "MapData.h"
#include "Map.h"
#include "MessageTypeEnum.h"
#include "MapElement.h"
#include "ErrorEnum.h"

namespace map_server
{
    void ElementInfoRequest::execute()
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        Map *map = mapData->getMap(_mapId);
        if (map != 0)
        {
            const MapElement *element = map->getElement(_elementId);
            if (element != 0)
            {
                std::string info = element->getInfoJson();
                MapData::unlock();

                _coutMutexPtr->lock();
                std::cout << _socketId << " " << _requestId << " " << map_server::ELEMENT_INFO << " " << info << std::endl;
                _coutMutexPtr->unlock();
            }
            else
            {
                MapData::unlock();

                _coutMutexPtr->lock();
                std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
                    << ",\"message\":\"Unknown element id ('" << _elementId << "') in ELEMENT_INFO request\"}" << std::endl;
                _coutMutexPtr->unlock();
            }

            flushErrors(map);
        }
        else
        {
            MapData::unlock();

            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
                << ",\"message\":\"Unknown map id ('" << _mapId << "') in ELEMENT_INFO request\"}" << std::endl;
            _coutMutexPtr->unlock();
        }
    }
}
