#include "GetItemDataRequest.h"
#include "MapData.h"
#include "Map.h"
#include "MessageTypeEnum.h"
#include "FilledPolygonItem.h"
#include "LineItem.h"
#include "PointItem.h"

namespace map_server
{
    void GetItemDataRequest::execute()
    {
        std::string info;
        MapData::lock();
        MapData *mapData = MapData::instance();

        Map *map = mapData->getMap(_mapId);
        if (map != 0 && _sendResponse)
        {
            const MapItem *item = map->getItem(_itemId);

            const FilledPolygonItem *filledPolygonItem = dynamic_cast<const FilledPolygonItem *>(item);
            if (filledPolygonItem != 0)
            {
                info = filledPolygonItem->getInfoJson(_resolutionIndex);
                MapData::unlock();

                _coutMutexPtr->lock();
                std::cout << _socketId << " " << _requestId << " " << map_server::GET_ITEM_DATA << " " << info << std::endl;
                _coutMutexPtr->unlock();
            }
            else
            {
                const LineItem *lineItem = dynamic_cast<const LineItem *>(item);
                if (lineItem != 0)
                {
                    info = lineItem->getInfoJson(_resolutionIndex);
                    MapData::unlock();

                    _coutMutexPtr->lock();
                    std::cout << _socketId << " " << _requestId << " " << map_server::GET_ITEM_DATA << " " << info << std::endl;
                    _coutMutexPtr->unlock();
                }
                else
                {
                    const PointItem *pointItem = dynamic_cast<const PointItem *>(item);
                    if (pointItem != 0)
                    {
                        info = pointItem->getInfoJson(_resolutionIndex);
                        MapData::unlock();

                        _coutMutexPtr->lock();
                        std::cout << _socketId << " " << _requestId << " " << map_server::GET_ITEM_DATA << " " << info << std::endl;
                        _coutMutexPtr->unlock();
                    }
                    else
                    {
                        MapData::unlock();
                    }
                }
            }
        }
        else
        {
            MapData::unlock();
        }
    }
}
