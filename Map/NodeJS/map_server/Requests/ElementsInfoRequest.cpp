#include "ElementsInfoRequest.h"
#include "MapData.h"
#include "Map.h"
#include "MessageTypeEnum.h"
#include "MapElement.h"

namespace map_server
{
    void ElementsInfoRequest::execute()
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        Map *map = mapData->getMap(_mapId);
        if (map != 0)
        {
            std::vector<std::string> infoVector;
            unsigned int i, n = _elementIds.size();
            for (i = 0; i < n; ++i)
            {
                const MapElement *element = map->getElement(_elementIds[i]);
                if (element != 0) infoVector.push_back(element->getInfoJson());
                else break;
            }

            if (infoVector.size() == n)
            {
                std::string info = "[";
                for (i = 0; i < n; ++i)
                {
                    info += infoVector[i];
                    if (i != n-1) info += ",";
                    else info += "]";
                }

                MapData::unlock();

                _coutMutexPtr->lock();
                std::cout << _socketId << " " << _requestId << " " << map_server::ELEMENTS_INFO << " " << info << std::endl;
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
