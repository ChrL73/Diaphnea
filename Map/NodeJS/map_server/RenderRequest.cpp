
#include "RenderRequest.h"
#include "MapData.h"
#include "Map.h"
#include "RequestTypeEnum.h"
#include "MapElement.h"

namespace map_server
{
    void RenderRequest::execute()
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        Map *map = mapData->getMap(_mapId);
        if (map != 0 && _sendResponse)
        {

            MapData::unlock();
        }
        else
        {
            MapData::unlock();
        }
    }
}
